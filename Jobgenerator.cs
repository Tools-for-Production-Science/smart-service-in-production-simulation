﻿using System;
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
        //static StreamWriter checkJobs = new StreamWriter("checkJobs.csv");
        static int seed = 42;
        // Generiert die benötigenten Aufträge und speichert diese in einer Liste ab
        public static (List<Job>, List<Job>) JobgeneratorS(Dictionary<string, double> inputData)
        {
            List<Job> Jobs1 = new List<Job>();
            List<Job> Jobs2 = new List<Job>();
            env = new Simulation(randomSeed: seed);
            seed += 567;
            for (int j = 1; j <= inputData["NumberOfJobs"]; j = 1+j)
            {
                List<Producttype> Producttype1 = new List<Producttype>();
                List<Producttype> Producttype2 = new List<Producttype>();

                for (int t = 1; t <= inputData["NumberPosition"]; t++)
                {
                    List<Product> Products1 = new List<Product>();
                    List<Product> Products2 = new List<Product>();
                    int ProductQuantity;

                    do
                    {
                        ProductQuantity = (int)env.RandLogNormal2(inputData[$"QuantityMean{t}"], inputData[$"QuantitySigma{t}"]);
                    } while (ProductQuantity <= 0);
                    //checkJobs.WriteLine(ProductQuantity);
                    double MaterialCost = inputData[$"MaterialCostsPerProductMean{t}"];

                    for (int p = 1; p <= ProductQuantity; p++)
                    {
                        Products1.Add(new Product(
                            p,
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
                            p,
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
                        TimeSpan.FromDays(inputData[$"SetupTimeMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimeSigma{t}"]),
                        MaterialCost,
                        inputData[$"Price{t}"]
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
                        TimeSpan.FromDays(inputData[$"SetupTimeMean{t}"]),
                        TimeSpan.FromDays(inputData[$"SetupTimeSigma{t}"]),
                        MaterialCost,
                        inputData[$"Price{t}"]
                        ));
                }

                Jobs1.Add(new Job(j, j, Producttype1));
                Jobs2.Add(new Job(j, j, Producttype2));
            }
            
            return (Jobs1, Jobs2);
        }
    }
}
