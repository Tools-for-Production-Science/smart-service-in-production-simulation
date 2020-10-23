using System;
using System.Collections.Generic;
using System.Text;
using SimSharp;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ProduktionssystemSimulation
{
    public static class Jobgenerator
    {  
        private static Simulation env;

        // Generiert die benötigenten Aufträge und speichert diese in einer Liste ab
        public static (List<Job>, List<Job>) JobgeneratorS(Dictionary<string, double> inputData, int seed)
        {
            List<Job> Jobs1 = new List<Job>();
            List<Job> Jobs2 = new List<Job>();
            env = new Simulation(randomSeed: seed);

            for (int i = 0; i < inputData["NumberOfJobs"]; i++)
            {
                List<Producttype> Positions1 = new List<Producttype>();
                List<Producttype> Positions2 = new List<Producttype>();
                for (int k = 1; k <= inputData["NumberPosition"]; k++)
                {
                    List<Product> Products1 = new List<Product>();
                    List<Product> Products2 = new List<Product>();
                    int ProductQuantity;

                    do
                    {
                        ProductQuantity = (int)env.RandLogNormal2(inputData[$"QuantityMean{k}"], inputData[$"QuantitySigma{k}"]);
                    } while (ProductQuantity <= 0);

                    double MaterialCost = inputData[$"MaterialCostsPerProductMean{k}"];

                    for (int j = 1; j <= ProductQuantity; j++)
                    {
                        Products1.Add(new Product(
                            j,
                            TimeSpan.FromDays(inputData["ProductionTimeProductPreMean"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductMainMean"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductPostMean"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductPreSigma"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductMainSigma"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductPostSigma"]),
                            TimeSpan.FromDays(inputData["ReworkTimeMean"]),
                            TimeSpan.FromDays(inputData["ReworkTimeSigma"])
                            ));

                        Products2.Add(new Product(
                            j,
                            TimeSpan.FromDays(inputData["ProductionTimeProductPreMean"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductMainMean"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductPostMean"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductPreSigma"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductMainSigma"]),
                            TimeSpan.FromDays(inputData["ProductionTimeProductPostSigma"]),
                            TimeSpan.FromDays(inputData["ReworkTimeMean"]),
                            TimeSpan.FromDays(inputData["ReworkTimeSigma"])
                            ));
                    }

                    Positions1.Add(new Producttype(
                        ProductQuantity,
                        k,
                        Products1,
                        inputData[$"ScrapRatioPreMean{k}"],
                        inputData[$"ScrapRatioMainMean{k}"],
                        inputData[$"ScrapRatioPostMean{k}"],
                        inputData[$"ReworkRatioPreMean{k}"],
                        inputData[$"ReworkRatioMainMean{k}"],
                        inputData[$"ReworkRatioPostMean{k}"],
                        TimeSpan.FromDays(inputData[$"SetupTimeMean{k}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimeSigma{k}"]),
                        MaterialCost,
                        inputData[$"Price{k}"]
                        ));

                    Positions2.Add(new Producttype(
                        ProductQuantity,
                        k,
                        Products1,
                        inputData[$"ScrapRatioPreMean{k}"],
                        inputData[$"ScrapRatioMainMean{k}"],
                        inputData[$"ScrapRatioPostMean{k}"],
                        inputData[$"ReworkRatioPreMean{k}"],
                        inputData[$"ReworkRatioMainMean{k}"],
                        inputData[$"ReworkRatioPostMean{k}"],
                        TimeSpan.FromDays(inputData[$"SetupTimeMean{k}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimeSigma{k}"]),
                        MaterialCost,
                        inputData[$"Price{k}"]
                        ));
                }

                Jobs1.Add(new Job(i, i + 1, Positions1));
                Jobs2.Add(new Job(i, i + 1, Positions2));
            }
            return (Jobs1, Jobs2);
        }
    }
}
