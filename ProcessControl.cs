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

        public IEnumerable<Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                yield return env.Process(Setup(env, mPre, mMain, mPost, job));
            }
            yield break;
        }
        public IEnumerable<Event> Setup(Simulation env, Resource mPre, Resource mMain, Resource mPost, Job job)
        {
            StoreGet getPipe;
            
            foreach (Producttype position in job.Producttype)
            {
                producedProducts = new Store(env, (int)position.Quantity);
                productsToProduce = new Store(env);
                producedQuanity = 0;
                position.TotalProducedQuantity = 0;
                //SetUp
                TimeSpan setupPre = env.RandLogNormal2(position.SetupMean, position.SetupSigma);
                analysis.SetupTimePre = analysis.SetupTimePre.Add(setupPre); 
                TimeSpan setupMain = env.RandLogNormal2(position.SetupMean, position.SetupSigma);
                analysis.SetupTimeMain = analysis.SetupTimeMain.Add(setupMain);
                TimeSpan setupPost = env.RandLogNormal2(position.SetupMean, position.SetupSigma);
                analysis.SetupTimePost = analysis.SetupTimePost.Add(setupPost);
                yield return env.Timeout(setupPre) & env.Timeout(setupMain) & env.Timeout(setupPost);
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
            Request reqPre = mPre.Request();
            yield return reqPre;
            yield return processPre = env.Process(preprocess.ProductionStep(mPre, reqPre, product));
            if (!product.Broken) { yield return env.Process(ReviewRework.ReviewPre(env, position, product, analysis));}
            if (product.Broken)
            {
                product.Broken = false;
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

        public IEnumerable<Event> BreakMachinePreprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(mtbfPre);
                yield return env.Timeout(failure);
                if (processPre != null && !brokenPre)
                {
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
