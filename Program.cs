﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Program
    {
        static int  i = 0;
        static String[] szenario = new String[] { "alle", "Scrap_Rework", "MTBF_Downtime" };
        static int szenarioID = 2;
        private static List<Double> GewinnSS = new List<Double>();
        private static List<Double> GewinnOSS = new List<Double>();
        //private static List<Position> Positions = new List<Position>();
        //private static List<Product> Products = new List<Product>();
        //private static readonly List<Job> Jobs = new List<Job>();
        private static SmartService SmartService;
        static Dictionary<string, double> inputData = new Dictionary<string, double>();
        private static Simulation env = new Simulation(randomSeed: 42);
        //private static int ProductQuantity;
        //private static double MaterialCost;

        static void Main(string[] args)
        {
            //Console.SetOut(TextWriter.Null);
            //Console.SetError(TextWriter.Null);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string curFile = @"C:\Users\kfausel\Documents\Simulation_BA\data_" + szenario[szenarioID] + ".txt";
            if (File.Exists(curFile))
            {
                Console.WriteLine("File exists.");
            }
            else
            {
                Console.WriteLine("File does not exist.");
                System.Environment.Exit(0);
            }
            foreach (string line in File.ReadAllLines(curFile, Encoding.UTF8))
            {
                string[] keyvalue = line.Split('=');
                if (keyvalue.Length == 2)
                {
                    inputData.Add(keyvalue[0], double.Parse(keyvalue[1], CultureInfo.InvariantCulture));
                }
            }
           
            //StreamWriter jobsCSV = new StreamWriter("Jobs.txt");
            //foreach(Job i in Jobs)
            //{
            //    jobsCSV.WriteLine(i.ToString() + "\n");
            //}
            SmartService = new SmartService(
                inputData["Scrap"],
                inputData["MTBFMean"],
                inputData["DowntimeMean"],
                inputData["DowntimeSigma"],
                inputData["Rework"]
            );
            // mit SS
            int seed = 0;
            StreamWriter swKPISS = new StreamWriter("KPISS_"+szenario[szenarioID]+".csv");

            //List<Job> Jobs = Jobgenerator();
            //Console.WriteLine("Jobs1 == Jobs2: " + EqualityComparer<List<Job>>.Default.Equals(Jobs1, Jobs1));
            List<List<Job>> JobsListe = new List<List<Job>>();
            //for(int i = 0; i< inputData["Rounds"]; i++)
            //{
            //    JobsListe.Add(Jobgenerator());
            //}
            swKPISS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");

            while (i < inputData["Rounds"])
            {
                //randomSeed: 42
                ProcessControl pc = new ProcessControl(Jobgenerator(), SmartService, inputData, new Simulation(randomSeed: seed));
                var tupelKPIProfit = pc.Simulate();
                GewinnSS.Add(tupelKPIProfit.Item2);
                //swGSS.WriteLine(tupelKPIProfit.Item2);
                foreach (var pair in tupelKPIProfit.Item1)
                {
                    swKPISS.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost"){
                        swKPISS.Write("{0}\n", tupelKPIProfit.Item2);
                    }
                }
                Console.WriteLine(i);

                i++;
                seed += 33;
            }

            swKPISS.Close();
            
            // ohne SS
            Console.WriteLine("SS ist fertig.");
            i = 0;
            SmartService = new SmartService(
                0,
                0,
                0,
                0,
                0
            );
            seed = 0;
            StreamWriter swKPIOSS = new StreamWriter("KPIOSS" + szenario[szenarioID] + ".csv");

            env = new Simulation(randomSeed: 42);
            swKPIOSS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");

            while (i < inputData["Rounds"])
            {
                //randomSeed: 42
                ProcessControl pc = new ProcessControl(Jobgenerator(), SmartService, inputData, new Simulation(randomSeed: seed));
                var tupelKPIProfit = pc.Simulate();
                GewinnOSS.Add(tupelKPIProfit.Item2);
                //swGOSS.WriteLine(tupelKPIProfit.Item2);
                foreach (var pair in tupelKPIProfit.Item1)
                {
                    swKPIOSS.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost")
                    {
                        swKPIOSS.Write("{0}\n", tupelKPIProfit.Item2);
                    }
                }

                Console.WriteLine(i);

                i++;
                seed += 33;
            }
            swKPIOSS.Close();

           
            StreamWriter Vorteil = new StreamWriter("GeldwerterVorteil" + szenario[szenarioID] + ".csv");
            double averageSS = GewinnSS.Average();
            double averageOSS = GewinnOSS.Average();
            double profit = averageSS - averageOSS;
            Console.WriteLine(profit);
            Vorteil.WriteLine("Gewinn Average SS: {0}", averageSS);
            Vorteil.WriteLine("Gewinn Average OSS: {0}", averageOSS);
            Vorteil.WriteLine("Geldwerter Vorteil: {0}", profit);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Vorteil.Close();
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        public static List<Job> Jobgenerator()
        {
            List<Job> Jobs = new List<Job>();
            for (int i = 0; i < 150; i++)
            {
                List<Position> Positions = new List<Position>();
                for (int k = 1; k <= inputData["NumberPosition"]; k++)
                {
                    List<Product> Products = new List<Product>();
                    int ProductQuantity = 0;
                    do
                    {
                        ProductQuantity = (int)env.RandLogNormal2(inputData[$"QuantityMean{k}"], inputData[$"QuantitySigma{k}"]);
                    } while (ProductQuantity <= 0);

                    double MaterialCost = 15;
                    //MaterialCost = env.RandNormalPositive(inputData[$"MaterialCostsPerProductMean{k}"], inputData[$"MaterialCostsPerProductSigma{k}"]);
                    // Verteilung um Anzahl der Produkte festzulegen
                    for (int j = 1; j <= ProductQuantity; j++)
                    {
                        Products.Add(new Product(
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

                    Positions.Add(new Position(
                        ProductQuantity,
                        k,
                        Products,
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

                Jobs.Add(new Job(i, i + 1, Positions));
            }
            return Jobs;
        }

        public static List<Job> CopyJobs(List<Job> oldJobs)
        {
            List<Job> Jobs = new List<Job>();
            for (int i = 0; i < oldJobs.Count; i++)
            {
                List<Position> Positions = new List<Position>();
                for (int k = 1; k <= inputData["NumberPosition"]; k++)
                {
                    List<Product> Products = new List<Product>();
                    
                    int ProductQuantity = oldJobs.ElementAt(i).Positions.ElementAt(k-1).Quantity;
                    double MaterialCost = oldJobs.ElementAt(i).Positions.ElementAt(k-1).MaterialCost;
                    //MaterialCost = env.RandNormalPositive(inputData[$"MaterialCostsPerProductMean{k}"], inputData[$"MaterialCostsPerProductSigma{k}"]);
                    // Verteilung um Anzahl der Produkte festzulegen
                    for (int j = 1; j <= ProductQuantity; j++)
                    {
                        Products.Add(new Product(
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

                    Positions.Add(new Position(
                        ProductQuantity,
                        k,
                        Products,
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

                Jobs.Add(new Job(i, i + 1, Positions));
            }
            return Jobs;
        }
    }
}
