using System;
using System.Collections.Generic;
using SimSharp;
using System.Collections;
using System.IO;

namespace ProductionsystemSimulation
{
    /*
     * 
     * This class controls the simulation
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
            mtbfMain = TimeSpan.FromDays(inputData["MTBFMain"] * (1 + smartService.smartServiceEffectMTBFMean));
            mtbfPost = TimeSpan.FromDays(inputData["MTBFPost"]);
            this.inputData = inputData;
            this.env = env;
        }

        // This method takes an order and starts the armor first before production starts
        public IEnumerable<Event> JobInProcess(Simulation env, Resource mPre, Resource mMain, Resource mPost, List<Job> jobs)
        {
            PriorityStore priorityStore = new PriorityStore(env);
            foreach (Job j in jobs)
            {
                priorityStore.Put(j, j.Priority);                
                   
            }
            while(priorityStore.Count != 0)
            {
                yield return env.Process(Setup(env, mPre, mMain, mPost, (Job)priorityStore.Get().Value));
            }
            
            yield break;
        }

        // Set up the machines and start production for each product
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
                //  Setup
                TimeSpan setupPre = env.RandLogNormal2(position.SetupPreMean, position.SetupPreSigma);
                analysis.SetupTimePre = analysis.SetupTimePre.Add(setupPre); 
                TimeSpan setupMain = env.RandLogNormal2(position.SetupMainMean, position.SetupMainSigma);
                analysis.SetupTimeMain = analysis.SetupTimeMain.Add(setupMain);
                TimeSpan setupPost = env.RandLogNormal2(position.SetupPostMean, position.SetupPostSigma);
                analysis.SetupTimePost = analysis.SetupTimePost.Add(setupPost);
                yield return env.Timeout(setupPre) & env.Timeout(setupMain) & env.Timeout(setupPost);

                // Each product is stored in a store so that if there is scrap, it is added back to the store and can be produced again
                foreach (Product product in position.Products)
                {
                    productsToProduce.Put(product);
                }

                // Produce the order until the target quantity is reached 
                while (producedQuanity != position.Quantity)
                {
                    // Take product out of store
                    getPipe = productsToProduce.Get();
                    // Waiting until a product is in the store or until all have been produced
                    yield return getPipe | producedProducts.WhenFull();
                    if (producedProducts.Count == position.Quantity)
                    {
                        break;
                    }
                    Product product = (Product)getPipe.Value;
                    position.TotalProducedQuantity += 1;
                    // no yield return here because don't want a product to go through all the machines first,
                    // but want them to go through the process "at the same time"
                    // so that when one product is finished the machine immediately makes the next one
                    env.Process(Production(env, mPre, mMain, mPost, position, product, productsToProduce));
                } 
            }
            finishedJobs.Add(job);
            analysis.JobExecutionTime = analysis.JobExecutionTime.Add(env.Now.Subtract(start));
            yield break;
        }

        // The actual manufacturing
        // From here, the machines are requested if they are available
        // and the machining step/process is started and subsequent verification
        public IEnumerable<Event> Production(Simulation env, Resource mPre, Resource mMain, Resource mPost, Producttype position, Product product, Store productsToProduce)
        {
            // Request machine according to availability
            Request reqPre = mPre.Request();
            // Wait until machine available
            yield return reqPre;
            // Start process and wait until it is finished before calling the verification
            yield return processPre = env.Process(preprocess.ProductionStep(mPre, reqPre, product));
            // The product may be broken if the machine fails
            // If product is broken because of the machine failure, it does not need to be checked and can be sorted out immediately
            if (!product.Broken) { yield return env.Process(ReviewRework.ReviewPre(env, position, product, analysis));}
            if (product.Broken)
            {
                product.Broken = false;
                productsToProduce.Put(product);
                yield break;
            }
            else
            {
                // If the product is not broken, the next step is executed.
                // Again, first request the availability
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

        // A process can be interrupted by calling Interrupt()
        // Therefore the pre, main and post process is stored above in Production() in a variable, with the currently running process
        // A separate method is needed for each machine, because always the one with this variable must be interrupted
        // By means of this the process can be interrupted
        // The IsOK flag of the process is set to false and the HandleFaulte() is called in the running process (here in the preprocess)
        public IEnumerable<Event> BreakMachinePreprocess(Simulation env)
        {
            while (true)
            {
                TimeSpan failure = env.RandExponential(mtbfPre);
                yield return env.Timeout(failure);
                if (processPre != null && !brokenPre)
                {
                    // Of course, it can happen that no process exists at the moment, but it is time for the machine to fail
                    // Then it is tried to interrupt the process until one exists which can also be interrupted
                    // Is needed, if for example the machines in front are slower and therefore a high idle time exists
                    // So the machining process must be started, so that the machine can fail
                    // It cannot fail while idle
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

        // Starts the simulation
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
