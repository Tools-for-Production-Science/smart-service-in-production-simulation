using System;
using System.Collections.Generic;
using SimSharp;

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

        public IEnumerable<Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                env.Log("----------------------  START NEW JOB  ----------------------");
                env.Log("JOB ID: {0}", job.ID);
                yield return env.Process(Setup(env, mPre, mMain, mPost, job));
            }
        }

        public IEnumerable<Event> Setup(Simulation env, Resource mPre, Resource mMain, Resource mPost, Job job)
        {
            foreach (Position position in job.Positions)
            {
                ReworkQuantity = 0;
                ReproductionQuantity = 0;
                //SetUp
                env.Log("---- SETUP PRODUCTYPE {0} ----", position.ID);
                yield return env.Timeout(position.Setup);

                foreach (Product product in position.Products)
                {
                    if (product.ID < position.Quantity)
                    {
                        env.Process(Production(env, mPre, mMain, mPost, product, position));
                    }
                    else
                    {
                        yield return env.Process(Production(env, mPre, mMain, mPost, product, position));
                    }
                }

                if (ReworkQuantity > 0)
                {
                    env.Log("START REWORK");
                    env.Process(ReviewRework.Rework(env, ReworkQuantity));
                }
                if (ReproductionQuantity > 0)
                {
                    int i = 1;
                    env.Log("START REPRODUCTION");
                    while (i <= ReproductionQuantity)
                    {
                        if(i < ReproductionQuantity )
                        {
                            env.Log("Reproduction of type {0}", i);
                            env.Process(Production(env, mPre, mMain, mPost, new Product(i), position));
                        } else
                        {
                            env.Log("Reproduction of type {0}", i);
                            yield return env.Process(Production(env, mPre, mMain, mPost, new Product(i), position));
                        }
                        i++;
                    }
                }
            }
        }

        public IEnumerable<Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Product product, Position position)
        {
            var arrive = env.Now;
            env.Log("{0} ProductNo {1}: Here I am", arrive, product.ID);
            var reqPre = mPre.Request();
            yield return reqPre;
            // var wait = env.Now - arrive;
            // env.Log("{0} {1}: waited {2}", env.Now, name, wait);
            yield return ProcessPre = env.Process(Preprocess.ProductionStep(env, mPre, reqPre, product.ID, position.ProductionTimePreProcess));
            var reqMain = mMain.Request();
            yield return reqMain;
            // var wait2 = env.Now - arrive;
            // env.Log("{0} {1}: waited {2}", env.Now, name, wait2);
            yield return ProcessMain = env.Process(Mainprocess.ProductionStep(env, mMain, reqMain, product.ID, position.sroductionTimeMainProcess));
            var reqPost = mPost.Request();
            yield return reqPost;
            // var wait3 = env.Now - arrive;
            yield return ProcessPost = env.Process(Postprocess.ProductionStep(env, mPost, reqPost, product.ID, position.ProductionTimePostProcess));
            var quantities = ReviewRework.Review(env, ReworkQuantity, ReproductionQuantity);
            ReworkQuantity = quantities.Item1;
            ReproductionQuantity = quantities.Item2;
        }

        public IEnumerable<Event> BreakMachinePreprocess(Simulation env)
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

        public IEnumerable<Event> BreakMachineMainprocess(Simulation env)
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
        public IEnumerable<Event> BreakMachinePostprocess(Simulation env)
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

            env.Run(TimeSpan.FromDays(3));
        }
    }
}
