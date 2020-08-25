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

        public ProcessControl(List<Job> jobs, SmartService smartService)
        {
            Jobs = jobs;
            SmartService = smartService;
            Preprocess = new Preprocess(env);
            Mainprocess = new Mainprocess(env, smartService);
            Postprocess = new Postprocess(env);
            MttfPre = TimeSpan.FromMinutes(300 * (1 + smartService.Effektausmaß * smartService.MaschinenAusfallwkeit));
            MttfMain = TimeSpan.FromMinutes(300 * (1 + smartService.Effektausmaß * smartService.MaschinenAusfallwkeit));
            MttfPost = TimeSpan.FromMinutes(300 * (1 + smartService.Effektausmaß * smartService.MaschinenAusfallwkeit));
        }

        public IEnumerable<SimSharp.Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs, Store pipe)
        {
            foreach (Job job in jobs)
            {
                env.Log("----------------------  START NEW JOB  ----------------------");
                env.Log("JOB ID: {0}", job.ID);
                yield return env.Process(Setup(env, mPre, mMain, mPost, job, pipe));
            }
        }
        int ProducedQuanity = 0;
        int i = 0;
        public IEnumerable<SimSharp.Event> Setup(Simulation env, Resource mPre, Resource mMain, Resource mPost, Job job, Store pipe)
        {
            StoreGet get;
            foreach (Position position in job.Positions)
            {
                ProducedQuanity = 0;
                i = 0;
                //SetUp
                env.Log("---- SETUP PRODUCTYPE {0} ----", position.ID);
                yield return env.Timeout(position.SetupPreMean);

                foreach (Product product in position.Products)
                {
                    pipe.Put(product);
                }
                Console.WriteLine(position.Products.Count);
                
                do
                {
                    get = pipe.Get();
                    yield return get;
                    Product product = (Product)get.Value;
                    if (i < position.Quantity)
                    {
 
                        env.Process(Production(env, mPre, mMain, mPost, product, pipe));
                    }else
                    {
                        yield return env.Process(Production(env, mPre, mMain, mPost, product, pipe));
                    }
                    i++;
                }
                while (ProducedQuanity < position.Quantity);
            }
        }

        public IEnumerable<SimSharp.Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Product product, Store pipe)
        {
            var arrive = env.Now;
            env.Log("{0} ProductNo {1}: Here I am", arrive, product.ID);
            var reqPre = mPre.Request();
            yield return reqPre;
            // var wait = env.Now - arrive;
            // env.Log("{0} {1}: waited {2}", env.Now, name, wait);
            yield return ProcessPre = env.Process(Preprocess.ProductionStep(env, mPre, reqPre, product));
            yield return env.Process(ReviewRework.ReviewPre(env, product));
            if (product.Broken)
            {
                product.Broken = false;
                env.Log("Start new production of product {0}", product.ID);
                pipe.Put(product);
               
            }
            else
            {
                var reqMain = mMain.Request();
                yield return reqMain;
                // var wait2 = env.Now - arrive;
                // env.Log("{0} {1}: waited {2}", env.Now, name, wait2);
                yield return ProcessMain = env.Process(Mainprocess.ProductionStep(env, mMain, reqMain, product));
                yield return env.Process(ReviewRework.ReviewMain(env, product));
                if (product.Broken)
                {
                    product.Broken = false;
                    env.Log("Start new production of product {0}", product.ID);
                    pipe.Put(product);
                    

                }
                else
                {
                    var reqPost = mPost.Request();
                    yield return reqPost;
                    // var wait3 = env.Now - arrive;
                    yield return ProcessPost = env.Process(Postprocess.ProductionStep(env, mPost, reqPost, product));
                    yield return env.Process(ReviewRework.ReviewPost(env, product));
                    if (product.Broken)
                    {
                        product.Broken = false;
                        env.Log("Start new production of product {0}", product.ID);
                        pipe.Put(product);
                        

                    }
                    else
                    {
                        ProducedQuanity++;
                        Console.WriteLine("Produced quantity: " + ProducedQuanity);
                    }
                    
                }
            }
        }

        public IEnumerable<SimSharp.Event> BreakMachinePreprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                var ausfall = env.RandExponential(MttfPost);
                env.Log("Pre ===================== {0}", ausfall);
                yield return env.Timeout(ausfall);
                if (ProcessPre != null && !BrokenPre && ProcessPre.IsOk && ProcessPre.IsAlive)
                {
                    env.Log("Break Machine Pre");
                    ProcessPre.Interrupt();
                }
            }
        }

        public IEnumerable<SimSharp.Event> BreakMachineMainprocess(Simulation env)
        {
            while (true)
            {
                var ausfall = env.RandExponential(MttfPost);
                env.Log("Main ===================== {0}", ausfall);
                yield return env.Timeout(ausfall);
                if (ProcessMain != null && !BrokenMain && ProcessMain.IsOk && ProcessMain.IsAlive)
                {
                    env.Log("Break Machine Main");
                    ProcessMain.Interrupt();   
                }
            }
        }
        public IEnumerable<SimSharp.Event> BreakMachinePostprocess(Simulation env)
        {
            while (true)
            {
                // Ausfallwahrscheinlichkeit für M
                var ausfall = env.RandExponential(MttfPost);
                env.Log("Post ===================== {0}", ausfall);
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
            var pipe = new Store(env);
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
            env.Process(JobInProcess(env, MPreprocess, MMainprocess, MPostprocess, Jobs, pipe));
            //env.Process(BreakMachinePreprocess(env));
            //env.Process(BreakMachineMainprocess(env));
            //env.Process(BreakMachinePostprocess(env));

            env.Run(TimeSpan.FromDays(2));
        }
    }
}
