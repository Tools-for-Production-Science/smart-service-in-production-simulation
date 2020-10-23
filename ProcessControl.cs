using System;
using System.Collections.Generic;
using SimSharp;
using System.Collections;
using System.IO;


namespace ProduktionssystemSimulation
{
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
        readonly private ArrayList finishedJobs = new ArrayList();
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
            mtbfMain = TimeSpan.FromDays(inputData["MTBFMain"] * (1 + smartService.MTBFMean));
            mtbfPost = TimeSpan.FromDays(inputData["MTBFPost"]);
            this.inputData = inputData;
            this.env = env;
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
                producedProducts = new Store(env, (int)position.Quantity);
                productsToProduce = new Store(env);
                producedQuanity = 0;
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
                    productsToProduce.Put(product);
                }

                while (producedQuanity != position.Quantity)
                {
                    getPipe = productsToProduce.Get();
                    // warte bis ein neues Teil produziert werden muss, oder alle Teile schon produziert sind
                    yield return getPipe | producedProducts.WhenFull();
                    if (producedProducts.Count == position.Quantity)
                    {
                        break;
                    }
                    Product product = (Product)getPipe.Value;
                    position.TotalProducedQuantity += 1;
                    env.Process(Production(env, mPre, mMain, mPost, position, product, productsToProduce));
                } 
            }
            finishedJobs.Add(job);
            yield break;
        }

        public IEnumerable<Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Producttype position, Product product, Store productsToProduce)
        {
            var arrive = env.Now;
            //env.Log("{0} ProductNo {1}: Here I am", arrive, product.ID);
            Request reqPre = mPre.Request();
            yield return reqPre;
            yield return processPre = env.Process(preprocess.ProductionStep(mPre, reqPre, product));
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
                yield return processMain = env.Process(mainprocess.ProductionStep( mMain, reqMain, product));
                if (!product.Broken)
                {
                    yield return env.Process(ReviewRework.ReviewMain(env, position, product, smartService, analysis));
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
                    yield return processPost = env.Process(postprocess.ProductionStep( mPost, reqPost, product));
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
                        producedProducts.Put(product);
                        producedQuanity++;
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
                TimeSpan failure = env.RandExponential(mtbfPost);
                //env.Log("{0}", failure);
                //analysis.ADOTPre = analysis.ADOTPre.Add(failure);
                yield return env.Timeout(failure);
                if (processPre != null && !brokenPre && processPre.IsOk && processPre.IsAlive)
                {
                    //env.Log("Break Machine Pre");
                    analysis.NumberOfFailurePre = analysis.NumberOfFailurePre +1;
                    processPre.Interrupt();
                }
            }
        }

        public IEnumerable<Event> BreakMachineMainprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(mtbfPost);
                //env.Log("{0}", failure);
                //analysis.ADOTMain = analysis.ADOTMain.Add(failure);
                yield return env.Timeout(failure);
                if (processMain != null && !brokenMain && processMain.IsOk && processMain.IsAlive)
                {
                    //env.Log("Break Machine Main");
                    analysis.NumberOfFailureMain = analysis.NumberOfFailureMain+1;
                    processMain.Interrupt();   
                }
            }
        }
        public IEnumerable<Event> BreakMachinePostprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                TimeSpan failure = env.RandExponential(mtbfPost);
                //env.Log("{0}",failure);
                //analysis.ADOTPost = analysis.ADOTPost.Add(failure);
                yield return env.Timeout(failure);
                if (processPost != null && !brokenPost && processPost.IsOk && processPost.IsAlive)
                {
                    //env.Log("Break Machine Post");
                    analysis.NumberOfFailurePost = analysis.NumberOfFailurePost+1;
                    processPost.Interrupt();
                }
            }
        }

        public (Dictionary<string, double>,double) Simulate()
        { 
            //Env.Log("======= Production Factory ========");
            
            Resource MPreprocess = new Resource(env, (int) inputData["CapacityPre"])
            {
                Utilization = new TimeSeriesMonitor(env, name: "M1 Utilization"),
                WaitingTime = new SampleMonitor(name: "M1 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(env, name: "M1 Queue Length", collect: true),
            };
            Resource MMainprocess = new Resource(env, (int)inputData["CapacityMain"])
            {
                Utilization = new TimeSeriesMonitor(env, name: "M2 Utilization"),
                WaitingTime = new SampleMonitor(name: "M2 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(env, name: "M2 Queue Length", collect: true),
            };
            Resource MPostprocess = new Resource(env, (int)inputData["CapacityPost"])
            {
                Utilization = new TimeSeriesMonitor(env, name: "M3 Utilization"),
                WaitingTime = new SampleMonitor(name: "M3 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(env, name: "M3 Queue Length", collect: true),
            };
            
            env.Process(JobInProcess(env, MPreprocess, MMainprocess, MPostprocess, jobs));
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
            env.Run(TimeSpan.FromHours(inputData["WorkingHours"]));
            analysis.FinishedJobs = finishedJobs;
            return (analysis.CalculateKPIs(), analysis.Profit());
            //FinishedJobs.ToList().ForEach(i => Console.WriteLine(i.ToString()));
        } 
    }
}
