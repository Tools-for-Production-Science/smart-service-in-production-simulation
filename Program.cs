using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ProduktionssystemSimulation
{
    class Program
    {
        private static List<Position> Positions = new List<Position>();
        private static List<Product> Products = new List<Product>();
        private static List<Job> Jobs = new List<Job>();
        private static SmartService SmartService;
        static Dictionary<string, double> result = new Dictionary<string, double>();
        static void Main(string[] args)
        {
            string curFile = @"F:\data.txt";
            if (File.Exists(curFile))
            {
                Console.WriteLine("File exists.");
            }
            else
            {
                Console.WriteLine("File does not exist.");
                Environment.Exit(0);
            }

            foreach (string line in File.ReadAllLines(curFile, Encoding.UTF8))
            {
                string[] keyvalue = line.Split('=');
                if (keyvalue.Length == 2)
                {
                    result.Add(keyvalue[0], double.Parse(keyvalue[1], CultureInfo.InvariantCulture));
                }
            }
           
            Jobgenerator();

            SmartService = new SmartService(
                result["ProbabilityOfOccurrence"], 
                result["Fixcosts"], 
                result["VariableCosts"], 
                TimeSpan.FromDays(result["Duration"]), 
                result["Price"], 
                result["MindCapture"], 
                result["MaxCapture"],
                result["InvestmentCosts"],
                result["Scrap"],
                result["MTTF"],
                result["Downtime"],
                result["Rework"]
            );
            ProcessControl pc = new ProcessControl(Jobs, SmartService, result);
            pc.Simulate();
        }

        public static void Jobgenerator()
        {
            for (int i = 0; i < 50; i++)
            {
                Positions = new List<Position>();
                for (int k = 1; k <= result["NumberPosition"]; k++)
                {
                    Products = new List<Product>();
                    // Verteilung um Anzahl der Produkte festzulegen
                    for (int j = 1; j <= 20; j++)
                    {
                        Products.Add(new Product(
                            j,
                            TimeSpan.FromDays(result["ProductionTimeProductPreMean"]),
                            TimeSpan.FromDays(result["ProductionTimeProductMainMean"]),
                            TimeSpan.FromDays(result["ProductionTimeProductPostMean"]),
                            TimeSpan.FromDays(result["ProductionTimeProductPreSigma"]),
                            TimeSpan.FromDays(result["ProductionTimeProductMainSigma"]),
                            TimeSpan.FromDays(result["ProductionTimeProductPostSigma"]),
                            TimeSpan.FromDays(result["ReworkTimeMean"]),
                            TimeSpan.FromDays(result["ReworkTimeSigma"])
                            ));
                    }

                    Positions.Add(new Position(
                        (int) result[$"QuantityMean{k}"],
                        (int) result[$"QuantitySigma{k}"],
                        k,
                        Products,
                        result[$"ScrapRatioPreMean{k}"],
                        result[$"ScrapRatioMainMean{k}"],
                        result[$"ScrapRatioPostMean{k}"],
                        result[$"ReworkRatioPreMean{k}"],
                        result[$"ReworkRatioMainMean{k}"],
                        result[$"ReworkRatioPostMean{k}"],
                        TimeSpan.FromDays(result[$"SetupTimePreMean{k}"]),
                        TimeSpan.FromDays(result[$"SetupTimePreSigma{k}"]),
                        TimeSpan.FromDays(result[$"SetupTimeMainMean{k}"]),
                        TimeSpan.FromDays(result[$"SetupTimeMainSigma{k}"]),
                        TimeSpan.FromDays(result[$"SetupTimePostMean{k}"]),
                        TimeSpan.FromDays(result[$"SetupTimePostSigma{k}"])
                        ));
                }

                Jobs.Add(new Job(i, i + 1, Positions));
            }
            Console.WriteLine("Anzahl Jobs: " + Jobs.Count);
        }
    }
}
