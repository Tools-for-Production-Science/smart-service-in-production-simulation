using System;
using System.Collections.Generic;
using SimSharp;
using System.Collections;
using System.IO;


namespace ProduktionssystemSimulation
{
    public class ProcessControl 
    {
        private Simulation Env;
        private List<Job> Jobs;
        private Preprocess Preprocess;
        private Mainprocess Mainprocess;
        private Postprocess Postprocess;
        private Process ProcessPre;
        private Process ProcessMain;
        private Process ProcessPost;
        public TimeSpan MtbfPre;
        public TimeSpan MtbfMain;
        public TimeSpan MtbfPost;
        public bool BrokenPre = false;
        public bool BrokenMain = false;
        public bool BrokenPost = false;
        int ProducedQuanity = 0;
        Store ProducedProducts;
        Store ProductsToProduce;
        Dictionary<string, double> InputData = new Dictionary<string, double>();
        private ArrayList FinishedJobs = new ArrayList();
        private Analysis analysis;
        private SmartService SmartService;

        public ProcessControl(List<Job> jobs, SmartService smartService, Dictionary<string, double> inputData, Simulation env)
        {
            Jobs = jobs;
            SmartService = smartService;
            analysis = new Analysis(inputData);
            Preprocess = new Preprocess(this, env, inputData["DowntimePreMean"], inputData["DowntimePreSigma"], analysis);
            Mainprocess = new Mainprocess(this, env, smartService, inputData["DowntimeMainMean"], inputData["DowntimeMainSigma"], analysis);
            Postprocess = new Postprocess(this, env, inputData["DowntimePostMean"], inputData["DowntimePostSigma"], analysis);
            MtbfPre = TimeSpan.FromDays(inputData["MTBFPre"]);
            MtbfMain = TimeSpan.FromDays(inputData["MTBFMain"] * (1 + smartService.MTBFMean));
            MtbfPost = TimeSpan.FromDays(inputData["MTBFPost"]);
            InputData = inputData;
            Env = env;
        }

        public IEnumerable<Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                //env.Log("----------------------  START NEW JOB  ----------------------");
                //env.Log("JOB ID: {0}", job.ID);
                yield return env.Process(Setup(env, mPre, mMain, mPost, job));
            }
            yield break;
        }
        public IEnumerable<Event> Setup(Simulation env, Resource mPre, Resource mMain, Resource mPost, Job job)
        {
            StoreGet getPipe;
            
            foreach (Producttype position in job.Positions)
            {
                ProducedProducts = new Store(env, (int)position.Quantity);
                ProductsToProduce = new Store(env);
                ProducedQuanity = 0;
                position.TotalProducedQuantity = 0;
                //Console.WriteLine(job.ID +" " + position.TotalProducedQuantity);

                //SetUp
                //env.Log("---- SETUP PRODUCTYPE {0} ----", position.ID);
                TimeSpan setupPre = env.RandLogNormal2(position.SetupMean, position.SetupSigma);
                analysis.AUSTPre = analysis.AUSTPre.Add(setupPre); 
                TimeSpan setupMain = env.RandLogNormal2(position.SetupMean, position.SetupSigma);
                analysis.AUSTMain = analysis.AUSTMain.Add(setupMain);
                TimeSpan setupPost = env.RandLogNormal2(position.SetupMean, position.SetupSigma);
                analysis.AUSTPost = analysis.AUSTPost.Add(setupPost);
                yield return env.Timeout(setupPre) & env.Timeout(setupMain) & env.Timeout(setupPost);
                //env.Log("End of setup");
                foreach (Product product in position.Products)
                {
                    ProductsToProduce.Put(product);
                }

                while (ProducedQuanity != position.Quantity)
                {
                    getPipe = ProductsToProduce.Get();
                    // warte bis ein neues Teil produziert werden muss, oder alle Teile schon produziert sind
                    yield return getPipe | ProducedProducts.WhenFull();
                    if (ProducedProducts.Count == position.Quantity)
                    {
                        break;
                    }
                    Product product = (Product)getPipe.Value;
                    position.TotalProducedQuantity += 1;
                    env.Process(Production(env, mPre, mMain, mPost, position, product, ProductsToProduce));
                } 
            }
            FinishedJobs.Add(job);
            yield break;
        }

        public IEnumerable<Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Producttype position, Product product, Store productsToProduce)
        {
            var arrive = env.Now;
            //env.Log("{0} ProductNo {1}: Here I am", arrive, product.ID);
            Request reqPre = mPre.Request();
            yield return reqPre;
            yield return ProcessPre = env.Process(Preprocess.ProductionStep(mPre, reqPre, product));
            if (!product.Broken) { yield return env.Process(ReviewRework.ReviewPre(env, position, product, analysis));}
            if (product.Broken)
            {
                product.Broken = false;
                //env.Log("Start new production of product {0}", product.ID);
                productsToProduce.Put(product);
                yield break;
            }
            else
            {
                var reqMain = mMain.Request();
                yield return reqMain;
                yield return ProcessMain = env.Process(Mainprocess.ProductionStep( mMain, reqMain, product));
                if (!product.Broken)
                {
                    yield return env.Process(ReviewRework.ReviewMain(env, position, product, SmartService, analysis));
                }
                if (product.Broken)
                {
                    product.Broken = false;
                    //env.Log("Start new production of product {0}", product.ID);
                    productsToProduce.Put(product);
                    yield break;
                }
                else
                {
                    var reqPost = mPost.Request();
                    yield return reqPost;
                    yield return ProcessPost = env.Process(Postprocess.ProductionStep( mPost, reqPost, product));
                    if (!product.Broken)
                    {
                        yield return env.Process(ReviewRework.ReviewPost(env, position, product, analysis));
                    }
                    if (product.Broken)
                    {
                        product.Broken = false;
                        //env.Log("Start new production of product {0}", product.ID);
                        productsToProduce.Put(product);
                        yield break;
                    }
                    else
                    {   
                        ProducedProducts.Put(product);
                        ProducedQuanity++;
                        //Console.WriteLine("Produced quantity: " + ProducedQuanity);
                    }
                }
            }
        }

        public IEnumerable<Event> BreakMachinePreprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                TimeSpan failure = env.RandExponential(MtbfPost);
                //env.Log("{0}", failure);
                //analysis.ADOTPre = analysis.ADOTPre.Add(failure);
                yield return env.Timeout(failure);
                if (ProcessPre != null && !BrokenPre && ProcessPre.IsOk && ProcessPre.IsAlive)
                {
                    //env.Log("Break Machine Pre");
                    analysis.NumberOfFailurePre = analysis.NumberOfFailurePre +1;
                    ProcessPre.Interrupt();
                }
            }
        }

        public IEnumerable<Event> BreakMachineMainprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(MtbfPost);
                //env.Log("{0}", failure);
                //analysis.ADOTMain = analysis.ADOTMain.Add(failure);
                yield return env.Timeout(failure);
                if (ProcessMain != null && !BrokenMain && ProcessMain.IsOk && ProcessMain.IsAlive)
                {
                    //env.Log("Break Machine Main");
                    analysis.NumberOfFailureMain = analysis.NumberOfFailureMain+1;
                    ProcessMain.Interrupt();   
                }
            }
        }
        public IEnumerable<Event> BreakMachinePostprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                TimeSpan failure = env.RandExponential(MtbfPost);
                //env.Log("{0}",failure);
                //analysis.ADOTPost = analysis.ADOTPost.Add(failure);
                yield return env.Timeout(failure);
                if (ProcessPost != null && !BrokenPost && ProcessPost.IsOk && ProcessPost.IsAlive)
                {
                    //env.Log("Break Machine Post");
                    analysis.NumberOfFailurePost = analysis.NumberOfFailurePost+1;
                    ProcessPost.Interrupt();
                }
            }
        }

        public (Dictionary<string, double>,double) Simulate()
        { 
            //Env.Log("======= Production Factory ========");
            
            Resource MPreprocess = new Resource(Env, (int) InputData["CapacityPre"])
            {
                Utilization = new TimeSeriesMonitor(Env, name: "M1 Utilization"),
                WaitingTime = new SampleMonitor(name: "M1 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(Env, name: "M1 Queue Length", collect: true),
            };
            Resource MMainprocess = new Resource(Env, (int)InputData["CapacityMain"])
            {
                Utilization = new TimeSeriesMonitor(Env, name: "M2 Utilization"),
                WaitingTime = new SampleMonitor(name: "M2 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(Env, name: "M2 Queue Length", collect: true),
            };
            Resource MPostprocess = new Resource(Env, (int)InputData["CapacityPost"])
            {
                Utilization = new TimeSeriesMonitor(Env, name: "M3 Utilization"),
                WaitingTime = new SampleMonitor(name: "M3 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(Env, name: "M3 Queue Length", collect: true),
            };
            
            Env.Process(JobInProcess(Env, MPreprocess, MMainprocess, MPostprocess, Jobs));
            if (MtbfPre != TimeSpan.FromDays(0))
            {
                Env.Process(BreakMachinePreprocess(Env));
            }
            if (MtbfMain != TimeSpan.FromDays(0))
            {
                Env.Process(BreakMachineMainprocess(Env));
            }
            if (MtbfPost != TimeSpan.FromDays(0))
            {
                Env.Process(BreakMachinePostprocess(Env));
            }
            
            FinishedJobs.Clear();
           
            //Env.Log(MPreprocess.Utilization.Summarize());
            //Env.Log(MPreprocess.WaitingTime.Summarize());
            //Env.Log(MPreprocess.QueueLength.Summarize());
            //Env.Log(MMainprocess.Utilization.Summarize());
            //Env.Log(MMainprocess.WaitingTime.Summarize());
            //Env.Log(MMainprocess.QueueLength.Summarize());
            //Env.Log(MPostprocess.Utilization.Summarize());
            //Env.Log(MPostprocess.WaitingTime.Summarize());
            //Env.Log(MPostprocess.QueueLength.Summarize());
            //var report = Report.CreateBuilder(Env)
            //  .Add("Utilization", MMainprocess.Utilization, Report.Measures.Mean | Report.Measures.StdDev)
            //  .Add("Queue", MMainprocess.QueueLength, Report.Measures.Min | Report.Measures.Mean | Report.Measures.Max)
            //  .Add("WaitingTime", MMainprocess.WaitingTime, Report.Measures.Min | Report.Measures.Mean | Report.Measures.Max)
            //  .SetOutput(new StreamWriter("report.csv")) // use a "new StreamWriter("report.csv")" to direct to a file
            //  .SetSeparator("\t")
            //  .SetPeriodicUpdate(TimeSpan.FromHours(20), withHeaders: false)
            //  .Build();
            //report.WriteHeader(); 
            Env.Run(TimeSpan.FromHours(InputData["WorkingHours"]));
            analysis.FinishedJobs = FinishedJobs;
            return (analysis.CalculateKPIs(), analysis.Profit());
            //FinishedJobs.ToList().ForEach(i => Console.WriteLine(i.ToString()));
        } 
    }
}
