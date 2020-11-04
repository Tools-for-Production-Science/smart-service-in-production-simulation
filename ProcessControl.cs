using System;
using System.Collections.Generic;
using SimSharp;
using System.Collections;
using System.IO;

namespace ProduktionssystemSimulation
{
    /*
     * 
     * Diese Klasse steuert die Simulation.
     * 
     */

    public class ProcessControl 
    {
        readonly private Simulation env;
        readonly private List<Job> jobs;
        readonly private Preprocess preprocess;
        readonly private Mainprocess mainprocess;
        readonly private Postprocess postprocess;
        private Process processPre;
        private Process processMain;
        private Process processPost;
        public TimeSpan mtbfPre;
        public TimeSpan mtbfMain;
        public TimeSpan mtbfPost;
        public bool brokenPre = false;
        public bool brokenMain = false;
        public bool brokenPost = false;
        int producedQuanity = 0;
        Store producedProducts;
        Store productsToProduce;
        readonly Dictionary<string, double> inputData = new Dictionary<string, double>();
        readonly private List<Job> finishedJobs = new List<Job>();
        readonly private Analysis analysis;
        readonly private SmartService smartService;

        public ProcessControl(List<Job> jobs, SmartService smartService, Dictionary<string, double> inputData, Simulation env)
        {
            this.jobs = jobs;
            this.smartService = smartService;
            analysis = new Analysis(inputData);
            preprocess = new Preprocess(this, env, inputData["DowntimePreMean"], inputData["DowntimePreSigma"], analysis);
            mainprocess = new Mainprocess(this, env, smartService, inputData["DowntimeMainMean"], inputData["DowntimeMainSigma"], analysis);
            postprocess = new Postprocess(this, env, inputData["DowntimePostMean"], inputData["DowntimePostSigma"], analysis);
            mtbfPre = TimeSpan.FromDays(inputData["MTBFPre"]);
            mtbfMain = TimeSpan.FromDays(inputData["MTBFMain"] * (1 + smartService.SSEffectMTBFMean));
            mtbfPost = TimeSpan.FromDays(inputData["MTBFPost"]);
            this.inputData = inputData;
            this.env = env;
        }

        // Diese Methode nimmt einen Auftrag und startet zunächst die Rüstung bevor die Produktion startet.
        public IEnumerable<Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                yield return env.Process(Setup(env, mPre, mMain, mPost, job));
            }
            yield break;
        }

        // Rüstung der Maschinen und Start der Produktion für jedes Produkt.
        public IEnumerable<Event> Setup(Simulation env, Resource mPre, Resource mMain, Resource mPost, Job job)
        {
            StoreGet getPipe;
            DateTime start = env.Now;

            foreach (Producttype position in job.Producttype)
            {
                producedProducts = new Store(env, (int)position.Quantity);
                productsToProduce = new Store(env);
                producedQuanity = 0;
                position.TotalProducedQuantity = 0;
                //  SetUp
                TimeSpan setupPre = env.RandLogNormal2(position.SetupPreMean, position.SetupPreSigma);
                analysis.SetupTimePre = analysis.SetupTimePre.Add(setupPre); 
                TimeSpan setupMain = env.RandLogNormal2(position.SetupMainMean, position.SetupMainSigma);
                analysis.SetupTimeMain = analysis.SetupTimeMain.Add(setupMain);
                TimeSpan setupPost = env.RandLogNormal2(position.SetupPostMean, position.SetupPostSigma);
                analysis.SetupTimePost = analysis.SetupTimePost.Add(setupPost);
                yield return env.Timeout(setupPre) & env.Timeout(setupMain) & env.Timeout(setupPost);

                // Jedes Produkt wird in ein Store gespeichert, damit falls Ausschuss ist, dies wieder dem Store hinzugefügt wird und nochmals produziert werden kann
                foreach (Product product in position.Products)
                {
                    productsToProduce.Put(product);
                }

                // Produzieren den Auftrag solange, bis die Soll-Menge erreicht ist 
                while (producedQuanity != position.Quantity)
                {
                    // Produkt aus Store nehmen.
                    getPipe = productsToProduce.Get();
                    // Warten bis ein Produkt im Store ist oder bis alle produziert wurden.
                    yield return getPipe | producedProducts.WhenFull();
                    if (producedProducts.Count == position.Quantity)
                    {
                        break;
                    }
                    Product product = (Product)getPipe.Value;
                    position.TotalProducedQuantity += 1;
                    // hier kein yield return, weil möchten nicht, dass ein Produkt erst alle Maschinen durchläuft, 
                    // sondern wollen, dass diese "gleichzeitig" den Prozess durchlaufen, sodass die Maschine nach fertigstellung eines Produktes sofort das nächte macht.
                    env.Process(Production(env, mPre, mMain, mPost, position, product, productsToProduce));
                } 
            }
            finishedJobs.Add(job);
            analysis.JobExecutionTime = analysis.JobExecutionTime.Add(env.Now.Subtract(start));
            yield break;
        }

        // Die eigentliche Herstellung.
        // Von hieraus werden die Maschinen angefragt, ob diese verfügbar sind und der Bearbeitungsschritt/Prozess gestartet
        // und anschließende Überprüfung.
        public IEnumerable<Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Producttype position, Product product, Store productsToProduce)
        {
            // Maschine nach Verfügbarkeit anfragen.
            Request reqPre = mPre.Request();
            // Warten bis Maschine verfügbar.
            yield return reqPre;
            // Prozess Starten und warten bis dieser fertig ist, bevor die Überprüfung aufgerufen wird
            yield return processPre = env.Process(preprocess.ProductionStep(mPre, reqPre, product));
            // Das Produkt kann kaputt gehen, wenn die Maschine Ausfällt. 
            // Wenn Produkt wegen dem Maschinenausfall kaputt geht, muss es nicht Überprüft werden und kann gleich Aussortiert werden.
            if (!product.Broken) { yield return env.Process(ReviewRework.ReviewPre(env, position, product, analysis));}
            if (product.Broken)
            {
                product.Broken = false;
                productsToProduce.Put(product);
                yield break;
            }
            else
            {
                // Wenn das Produkt nicht kaputt ist, wird der nächste Schritt ausgeführt. 
                // Auch hier wieder erster die Verfügbarkeit anfragen.
                var reqMain = mMain.Request();
                yield return reqMain;
                yield return processMain = env.Process(mainprocess.ProductionStep( mMain, reqMain, product));
                if (!product.Broken)
                {
                    yield return env.Process(ReviewRework.ReviewMain(env, position, product, smartService, analysis));
                }
                if (product.Broken)
                {
                    product.Broken = false;
                    productsToProduce.Put(product);
                    yield break;
                }
                else
                {
                    var reqPost = mPost.Request();
                    yield return reqPost;
                    yield return processPost = env.Process(postprocess.ProductionStep( mPost, reqPost, product));
                    if (!product.Broken)
                    {
                        yield return env.Process(ReviewRework.ReviewPost(env, position, product, analysis));
                    }
                    if (product.Broken)
                    {
                        product.Broken = false;
                        productsToProduce.Put(product);
                        yield break;
                    }
                    else
                    {   
                        producedProducts.Put(product);
                        producedQuanity++;
                    }
                }
            }
        }

        // Ein Prozess kann unterbrochen werden, indem Interrupt() aufgerufen wird. 
        // Daher wird der Vor-, Haupt- und Nachprozess obendrüber in Production() in einer Variable gespeichert, mit dem gerade laufenden Prozess.
        // Es wird für jede Maschine eine eigene Methode benötigt, da immer der mit dieser Variable unterbrochen werden muss.
        // Mittels dieser kann der Prozess unterbochen werden. 
        // Dabei wird die IsOK-Flag des Prozesses auf false gesetzt und die HandleFaulte() wird in dem laufenden Prozess (also hier im preprocess) aufgerufen.
        public IEnumerable<Event> BreakMachinePreprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(mtbfPre);
                yield return env.Timeout(failure);
                if (processPre != null && !brokenPre)
                {
                    // Es kann natürlich vorkammen, dass gerade kein Porzess existiert, aber es Zeit ist, dass die Maschine ausfällt. 
                    // Dann wird solange versucht den Prozess zu unterbrechen, bis einer existiert, der auch unterbrochen werden kann.
                    // Wird benötigt, wenn zum Beispiel die Maschinen vorne dran langersamer sind und somit eine hohe Leerlaufzeit existiert.
                    // Der Bearbeitungsprozess muss also gestartet sein, damit die Maschine ausfallen kann.
                    // Sie kann nicht im Leerlauf ausfallen.
                    do
                    {
                        if (processPre.IsOk && processPre.IsAlive)
                        {
                            analysis.NumberOfFailurePre = analysis.NumberOfFailurePre + 1;
                            processPre.Interrupt();
                            break;
                        }
                        else
                        {
                            yield return env.Timeout(TimeSpan.FromSeconds(1));
                            continue;
                        }
                    } while (true);
                }
            }
        }

        public IEnumerable<Event> BreakMachineMainprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(mtbfMain);
                yield return env.Timeout(failure);
                if (processMain != null && !brokenMain)
                {
                    do
                    {
                        if (processMain.IsOk && processMain.IsAlive)
                        {
                            analysis.NumberOfFailureMain = analysis.NumberOfFailureMain + 1;
                            processMain.Interrupt();
                            break;
                        }else
                        {
                            yield return env.Timeout(TimeSpan.FromSeconds(1));
                            continue;
                        }
                    } while(true);
                }
            }
        }

        public IEnumerable<Event> BreakMachinePostprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(mtbfPost);
                yield return env.Timeout(failure);
               
                if (processPost != null && !brokenPost )
                {
                    do
                    {
                        if (processPost.IsOk && processPost.IsAlive)
                        {
                            analysis.NumberOfFailurePost = analysis.NumberOfFailurePost + 1;
                            processPost.Interrupt();
                            break;
                        }
                        else
                        {
                            yield return env.Timeout(TimeSpan.FromSeconds(1));
                            continue;
                        }
                    } while (true);
                }
            }
        }

        // Startet die Simulation.
        public (Dictionary<string, double>,double, List<Job>) Simulate()
        {             
            Resource MPreprocess = new Resource(env, (int) inputData["CapacityPre"]);
            Resource MMainprocess = new Resource(env, (int)inputData["CapacityMain"]);
            Resource MPostprocess = new Resource(env, (int)inputData["CapacityPost"]);

            if (mtbfPre != TimeSpan.FromDays(0))
            {
                env.Process(BreakMachinePreprocess(env));
            }
            if (mtbfMain != TimeSpan.FromDays(0))
            {
                env.Process(BreakMachineMainprocess(env));
            }
            if (mtbfPost != TimeSpan.FromDays(0))
            {
                env.Process(BreakMachinePostprocess(env));
            }
            
            finishedJobs.Clear();
            env.Process(JobInProcess(env, MPreprocess, MMainprocess, MPostprocess, jobs));
            env.Run(TimeSpan.FromHours(inputData["WorkingHours"]));
            analysis.FinishedJobs = finishedJobs;
            return (analysis.CalculateKPIs(), analysis.Profit(), finishedJobs);
        } 
    }
}
