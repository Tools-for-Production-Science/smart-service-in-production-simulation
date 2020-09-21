using System;
using System.Collections.Generic;
using SimSharp;
using System.Collections;
using System.IO;

namespace ProduktionssystemSimulation
{
    class ProcessControl 
    {
        private static readonly Simulation  env = new Simulation(TimeSpan.FromMinutes(1));
        private static List<Job> Jobs;
        private static SmartService SmartService;
        private static Preprocess Preprocess;
        private static Mainprocess Mainprocess;
        private static Postprocess Postprocess;
        private Process ProcessPre;
        private Process ProcessMain;
        private Process ProcessPost;
        public static int ReworkQuantity = 0;
        public static int ReproductionQuantity = 0;
        public TimeSpan MttfPre;
        public TimeSpan MttfMain;
        public TimeSpan MttfPost;
        public static Boolean BrokenPre = false;
        public static Boolean BrokenMain = false;
        public static Boolean BrokenPost = false;
        int ProducedQuanity = 0;
        Store ProducedProducts;
        Store ProductsToProduce;
        static Dictionary<string, double> Result = new Dictionary<string, double>();

        public ProcessControl(List<Job> jobs, SmartService smartService, Dictionary<string, double> result)
        {
            Jobs = jobs;
            SmartService = smartService;
            Preprocess = new Preprocess(env, Result["DowntimePreMean"], Result["DowntimePreSigma"]);
            Mainprocess = new Mainprocess(env, smartService, Result["DowntimeMainMean"], Result["DowntimeMainSigma"]);
            Postprocess = new Postprocess(env, Result["DowntimePostMean"], Result["DowntimePostSigma"]);
            MttfPre = TimeSpan.FromMinutes(result["MTTFPre"]);
            MttfMain = TimeSpan.FromMinutes(result["MTTFMain"] * (1 + result["MTTF"]));
            MttfPost = TimeSpan.FromMinutes(result["MTTFPost"]);
            Result = result;
        }

        public IEnumerable<Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                env.Log("----------------------  START NEW JOB  ----------------------");
                env.Log("JOB ID: {0}", job.ID);
                yield return env.Process(Setup(env, mPre, mMain, mPost, job));
            }
            yield break;
        }
       
        public IEnumerable<Event> Setup(Simulation env, Resource mPre, Resource mMain, Resource mPost, Job job)
        {
            StoreGet getPipe;
            
            foreach (Position position in job.Positions)
            {
                ProducedProducts = new Store(env, position.QuantityMean);
                ProductsToProduce = new Store(env);
                ProducedQuanity = 0;
               
                //SetUp
                env.Log("---- SETUP PRODUCTYPE {0} ----", position.ID);
                yield return env.Timeout(position.SetupPreMean);

                foreach (Product product in position.Products)
                {
                    ProductsToProduce.Put(product);
                }

                while (ProducedQuanity != position.QuantityMean)
                {
                    getPipe = ProductsToProduce.Get();
                    // warte bis ein neues Teil produziert werden muss, oder alle Teile schon produziert sind
                    yield return getPipe | ProducedProducts.WhenFull();
                    if (ProducedProducts.Count == position.QuantityMean)
                    {
                        break;
                    }

                    Product product = (Product)getPipe.Value;

                    env.Process(Production(env, mPre, mMain, mPost, position, product, ProductsToProduce));
                } 
            }
            yield break;
        }

        public IEnumerable<Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Position position, Product product, Store pipe)
        {
            var arrive = env.Now;
            env.Log("{0} ProductNo {1}: Here I am", arrive, product.ID);
            var reqPre = mPre.Request();
            yield return reqPre;
            // var wait = env.Now - arrive;
            // env.Log("{0} {1}: waited {2}", env.Now, name, wait);
            yield return ProcessPre = env.Process(Preprocess.ProductionStep(env, mPre, reqPre, product));
            yield return env.Process(ReviewRework.ReviewPre(env, position, product));
            if (product.Broken)
            {
                product.Broken = false;
                env.Log("Start new production of product {0}", product.ID);
                pipe.Put(product);
                yield break;
            }
            else
            {
                var reqMain = mMain.Request();
                yield return reqMain;
                // var wait2 = env.Now - arrive;
                // env.Log("{0} {1}: waited {2}", env.Now, name, wait2);
                yield return ProcessMain = env.Process(Mainprocess.ProductionStep(env, mMain, reqMain, product));
                yield return env.Process(ReviewRework.ReviewMain(env, position, product, SmartService));
                if (product.Broken)
                {
                    product.Broken = false;
                    env.Log("Start new production of product {0}", product.ID);
                    pipe.Put(product);
                    yield break;
                }
                else
                {
                    var reqPost = mPost.Request();
                    yield return reqPost;
                    // var wait3 = env.Now - arrive;
                    yield return ProcessPost = env.Process(Postprocess.ProductionStep(env, mPost, reqPost, product));
                    yield return env.Process(ReviewRework.ReviewPost(env, position, product));
                    if (product.Broken)
                    {
                        product.Broken = false;
                        env.Log("Start new production of product {0}", product.ID);
                        pipe.Put(product);
                        yield break;
                    }
                    else
                    {
                        ProducedQuanity++;
                        ProducedProducts.Put(product);
                        Console.WriteLine("Produced quantity: " + ProducedQuanity);
                    }
                }
            }
        }

        public IEnumerable<Event> BreakMachinePreprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                var ausfall = env.RandExponential(MttfPost);
                yield return env.Timeout(ausfall);
                if (ProcessPre != null && !BrokenPre && ProcessPre.IsOk && ProcessPre.IsAlive)
                {
                    env.Log("Break Machine Pre");
                    ProcessPre.Interrupt();
                }
            }
        }

        public IEnumerable<Event> BreakMachineMainprocess(Simulation env)
        {
            while (true)
            {
                var ausfall = env.RandExponential(MttfPost);
                yield return env.Timeout(ausfall);
                if (ProcessMain != null && !BrokenMain && ProcessMain.IsOk && ProcessMain.IsAlive)
                {
                    env.Log("Break Machine Main");
                    ProcessMain.Interrupt();   
                }
            }
        }
        public IEnumerable<Event> BreakMachinePostprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                var ausfall = env.RandExponential(MttfPost);
                yield return env.Timeout(ausfall);
                if (ProcessPost != null && !BrokenPost && ProcessPost.IsOk && ProcessPost.IsAlive)
                {
                    env.Log("Break Machine Post");
                    ProcessPost.Interrupt();
                }
            }
        }

        public void Simulate()
        { 
            env.Log("======= Production Factory ========");
            
            Resource MPreprocess = new Resource(env, 1)
            {
                Utilization = new TimeSeriesMonitor(env, name: "M1 Utilization"),
                WaitingTime = new SampleMonitor(name: "M1 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(env, name: "M1 Queue Length", collect: true),
            };
            Resource MMainprocess = new Resource(env, 1)
            {
                Utilization = new TimeSeriesMonitor(env, name: "M2 Utilization"),
                WaitingTime = new SampleMonitor(name: "M2 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(env, name: "M2 Queue Length", collect: true),
            };
            Resource MPostprocess = new Resource(env, 1)
            {
                Utilization = new TimeSeriesMonitor(env, name: "M3 Utilization"),
                WaitingTime = new SampleMonitor(name: "M3 Waiting time", collect: true),
                QueueLength = new TimeSeriesMonitor(env, name: "M3 Queue Length", collect: true),
            };
            env.Process(JobInProcess(env, MPreprocess, MMainprocess, MPostprocess, Jobs));
            env.Process(BreakMachinePreprocess(env));
            env.Process(BreakMachineMainprocess(env));
            env.Process(BreakMachinePostprocess(env));

            env.Run(TimeSpan.FromDays(2));
        }
    }
}
