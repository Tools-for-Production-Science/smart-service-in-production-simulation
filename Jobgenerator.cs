using System;
using System.Collections.Generic;
using SimSharp;

namespace ProductionsystemSimulation
{
    /*
     * 
     * This class generates the required orders for the simulation
     * Two identical lists with the same orders are generated
     * 
     */
    public static class Jobgenerator
    {
        // Simulation environment is generated here, as this is required for the determination of the product quantity
        private static Simulation env;
        static int seed = 42;

        public static (List<Job>, List<Job>) GenerateJobs(Dictionary<string, double> inputData)
        {
            List<Job> Jobs1 = new();
            List<Job> Jobs2 = new();
            env = new Simulation(randomSeed: seed);
            seed += 567;
            for (int j = 1; j <= inputData["NumberOfJobs"]; j = 1+j)
            {
                List<Producttype> Producttype1 = new();
                List<Producttype> Producttype2 = new();

                for (int t = 1; t <= inputData["NumberPosition"]; t++)
                {
                    List<Product> Products1 = new();
                    List<Product> Products2 = new();
                    int ProductQuantity;

                    do
                    {
                        ProductQuantity = (int)env.RandLogNormal2(inputData[$"QuantityMean{t}"], inputData[$"QuantitySigma{t}"]);
                    } while (ProductQuantity <= 0);

                    double MaterialCost = inputData[$"MaterialCostsPerProductMean{t}"];

                    for (int p = 1; p <= ProductQuantity; p++)
                    {
                        Products1.Add(new Product(
                            p,
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPreMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductMainMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPostMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPreSigma{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductMainSigma{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPostSigma{t}"]),
                            TimeSpan.FromDays(inputData[$"ReworkTimeMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ReworkTimeSigma{t}"])
                            ));

                        Products2.Add(new Product(
                            p,
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPreMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductMainMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPostMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPreSigma{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductMainSigma{t}"]),
                            TimeSpan.FromDays(inputData[$"ProductionTimeProductPostSigma{t}"]),
                            TimeSpan.FromDays(inputData[$"ReworkTimeMean{t}"]),
                            TimeSpan.FromDays(inputData[$"ReworkTimeSigma{t}"])
                            ));
                    }

                    Producttype1.Add(new Producttype(
                        ProductQuantity,
                        t,
                        Products1,
                        inputData[$"ScrapRatioPreMean{t}"],
                        inputData[$"ScrapRatioMainMean{t}"],
                        inputData[$"ScrapRatioPostMean{t}"],
                        inputData[$"ReworkRatioPreMean{t}"],
                        inputData[$"ReworkRatioMainMean{t}"],
                        inputData[$"ReworkRatioPostMean{t}"],
                        TimeSpan.FromDays(inputData[$"SetupTimePreMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimePreSigma{t}"]),
                        MaterialCost,
                        inputData[$"Price{t}"],
                        TimeSpan.FromDays(inputData[$"SetupTimeMainMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimeMainSigma{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimePostMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimePostSigma{t}"])
                        ));

                    Producttype2.Add(new Producttype(
                        ProductQuantity,
                        t,
                        Products1,
                        inputData[$"ScrapRatioPreMean{t}"],
                        inputData[$"ScrapRatioMainMean{t}"],
                        inputData[$"ScrapRatioPostMean{t}"],
                        inputData[$"ReworkRatioPreMean{t}"],
                        inputData[$"ReworkRatioMainMean{t}"],
                        inputData[$"ReworkRatioPostMean{t}"],
                        TimeSpan.FromDays(inputData[$"SetupTimePreMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimePreSigma{t}"]),
                        MaterialCost,
                        inputData[$"Price{t}"],
                        TimeSpan.FromDays(inputData[$"SetupTimeMainMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimeMainSigma{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimePostMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimePostSigma{t}"])
                        ));
                }
                // The ID is also passed as a Prio, so that the order in which the jobs are processed is identical for the runs with and without Smart Service
                Jobs1.Add(new Job(j, j, Producttype1));
                Jobs2.Add(new Job(j, j, Producttype2));
            }
            
            return (Jobs1, Jobs2);
        }
    }
}
