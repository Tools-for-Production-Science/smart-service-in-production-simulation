using System;
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
            string curFile = @"F:\data.txt";
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
           
            StreamWriter jobsCSV = new StreamWriter("Jobs.txt");
            //foreach(Job i in Jobs)
            //{
            //    jobsCSV.WriteLine(i.ToString() + "\n");
            //}
            SmartService = new SmartService(
                inputData["Scrap"],
                inputData["MTTF"],
                inputData["Downtime"],
                inputData["Rework"]
            );
            // mit SS
            int seed = 0;
            StreamWriter swKPISS = new StreamWriter("KPISS.csv");
            StreamWriter swGSS = new StreamWriter("GewinnSS.csv");
            List<Job> Jobs = Jobgenerator();
            //Console.WriteLine("Jobs1 == Jobs2: " + EqualityComparer<List<Job>>.Default.Equals(Jobs1, Jobs1));
            while (i < inputData["Rounds"])
            {
                //randomSeed: 42
                ProcessControl pc = new ProcessControl(CopyJobs(Jobs), SmartService, inputData, new Simulation(randomSeed: seed));
                var tupelKPIProfit = pc.Simulate();
                GewinnSS.Add(tupelKPIProfit.Item2);
                swGSS.WriteLine(tupelKPIProfit.Item2);
                foreach (var i in tupelKPIProfit.Item1)
                {
                    swKPISS.WriteLine(i.ToString());
                }

                i++;
                seed += 33;
            }
            swGSS.Close();
            swKPISS.Close();
            // ohne SS
            i = 0;
            SmartService = new SmartService(
                0,
                0,
                0,
                0
            );
            seed = 0;
            StreamWriter swKPIOSS = new StreamWriter("KPIOSS.csv");
            StreamWriter swGOSS = new StreamWriter("GewinnOSS.csv");

            while (i < inputData["Rounds"])
            {
                //randomSeed: 42
                ProcessControl pc = new ProcessControl(CopyJobs(Jobs), SmartService, inputData, new Simulation(randomSeed: seed));
                var tupelKPIProfit = pc.Simulate();
                GewinnOSS.Add(tupelKPIProfit.Item2);
                swGOSS.WriteLine(tupelKPIProfit.Item2);
                foreach (var i in tupelKPIProfit.Item1)
                {
                    swKPIOSS.WriteLine(i.ToString());
                }
                i++;
                seed += 33;
            }
            swKPIOSS.Close();
            swGOSS.Close();
           
            StreamWriter Vorteil = new StreamWriter("GeldwerterVorteil.csv");
            double averageSS = GewinnSS.Average();
            double averageOSS = GewinnOSS.Average();
            double profit = averageSS - averageOSS;
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
            for (int i = 0; i < 1000; i++)
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
