using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SimSharp;

namespace ProductionsystemSimulation
{
    /*
     * 
     * This class contains the Main method
     * This class is responsible for the simulation runs
     * 
     */
    class Program
    {
        readonly static String[] szenario = new String[] { "", "_all", "_Scrap_Rework", "_MTBF_Downtime", "_Scrap_Rework_OneYear", "_Scrap_Rework_TwoYears", "_Scrap_Rework_ThreeYears" };
        static int szenarioID = 0;
        private static SmartService SmartService;
        readonly static Dictionary<string, double> inputData = new Dictionary<string, double>();
        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("SzenarioID: " + szenarioID);
            // Measure time how long the program takes
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Read in configuration file
            // different scenarios can be stored and specified in the array above with the ID the desired scenario can be selected
            string curFile = "data" + szenario[szenarioID] + ".txt";
            if (File.Exists(curFile))
            {
                Console.WriteLine("File exists.");
            }
            else
            {
                Console.WriteLine("File does not exist.");
                System.Environment.Exit(0);
            }

            // save Inputdata in Dictionary<string, double> 
            foreach (string line in File.ReadAllLines(curFile, Encoding.UTF8))
            {
                string[] keyvalue = line.Split('=');
                if (keyvalue.Length == 2)
                {
                    inputData.Add(keyvalue[0], double.Parse(keyvalue[1], CultureInfo.InvariantCulture));
                }
            }

            SmartService = new SmartService(
                inputData["SmartServiceEffectScrap"],
                inputData["SmartServiceEffectMTBFMean"],
                inputData["SmartServiceEffectDowntimeMean"],
                inputData["SmartServiceEffectDowntimeSigma"],
                inputData["SmartServiceEffectRework"]
            );

            SmartService withoutSmartService = new SmartService(
                0,
                0,
                0,
                0,
                0
            );

            StreamWriter outputSmartService = new StreamWriter("Output_smartService_" + szenario[szenarioID]+ ".csv");
            StreamWriter outputWithoutSmartService = new StreamWriter("Output_withoutSmartService_" + szenario[szenarioID] + ".csv");

            List<double> GewinnSmartService = new List<double>();
            List<double> GewinnOhneSmartService = new List<double>();
            List<List<Job>> saveJobsSmartService = new List<List<Job>>();
            List<List<Job>> saveJobsWithoutSmartService = new List<List<Job>>();

            // add CSV header
            outputSmartService.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");
            outputWithoutSmartService.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");

            int i = 0;


            // Simulation runs with and without Smart Service
            while (i < inputData["Iterations"])
            {
                (List<Job>, List<Job>) tupelListJobs = Jobgenerator.GenerateJobs(inputData);
                

                int seed = random.Next();

                // Simulation run with Smart Service
                ProcessControl pcSmartService = new ProcessControl(tupelListJobs.Item1, SmartService, inputData, new Simulation(randomSeed: seed));

                (Dictionary<string, double>, double, List<Job>) tupelKPIProfitSmartService = pcSmartService.Simulate();
                saveJobsSmartService.Add(tupelKPIProfitSmartService.Item3);

                // Save profit for pecuniary advantage so that the average can be calculated
                GewinnSmartService.Add(tupelKPIProfitSmartService.Item2);

                // Write KPIs and profit to CSV
                foreach (var pair in tupelKPIProfitSmartService.Item1)
                {
                    outputSmartService.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost"){
                        outputSmartService.Write("{0}\n", tupelKPIProfitSmartService.Item2);
                    }
                }

                // Simulation run without Smart Service
                ProcessControl pcWithoutSmartService = new ProcessControl(tupelListJobs.Item2, withoutSmartService, inputData, new Simulation(randomSeed: seed));

                (Dictionary<string, double>, double, List<Job>) tupelKPIProfitWithoutSmartService = pcWithoutSmartService.Simulate();
                saveJobsWithoutSmartService.Add(tupelKPIProfitWithoutSmartService.Item3);

                // Save profit for pecuniary advantage so that the average can be calculated
                GewinnOhneSmartService.Add(tupelKPIProfitWithoutSmartService.Item2);

                foreach (var pair in tupelKPIProfitWithoutSmartService.Item1)
                {
                    outputWithoutSmartService.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost")
                    {
                        outputWithoutSmartService.Write("{0}\n", tupelKPIProfitWithoutSmartService.Item2);
                    }
                }

                if (i == inputData["Iterations"]/4) Console.WriteLine("Quater.");
                if (i == inputData["Iterations"]/2) Console.WriteLine("Half time.");
                if (i == (inputData["Iterations"]*3)/4) Console.WriteLine("Three quarters.");
                Console.WriteLine(i);
                i++;
            }

            outputSmartService.Close();
            outputWithoutSmartService.Close();

            StreamWriter monetaryBenefit = new StreamWriter("GeldwerterVorteil_" + szenario[szenarioID] + ".csv");
            StreamWriter outputJobsSmartService = new StreamWriter("Output_Jobs_SmartService_" + szenario[szenarioID] + ".csv");
            StreamWriter outputJobsWithoutSmartService = new StreamWriter("Output_Jobs_OhneSmartService_" + szenario[szenarioID] + ".csv");

            outputJobsSmartService.WriteLine("Job ID; Producttype ID; Quantity");
            outputJobsWithoutSmartService.WriteLine("Job ID; Producttype ID; Quantity");

            // Write completed orders with Smart Service to CSV
            foreach (List<Job> list in saveJobsSmartService)
            {
                foreach (Job job in list)
                {
                    outputJobsSmartService.Write(job.ID + ";");

                    foreach (Producttype type in job.Producttype)
                    {
                        outputJobsSmartService.Write(type.ID + ";" + type.Quantity + "\n");
                    }

                }
            }

            // Write completed orders without Smart Service to CSV
            foreach (List<Job> list in saveJobsWithoutSmartService)
            {
                foreach (Job job in list)
                {
                    outputJobsWithoutSmartService.Write(job.ID + ";");

                    foreach (Producttype type in job.Producttype)
                    {
                        outputJobsWithoutSmartService.Write(type.ID + ";" + type.Quantity + "\n");

                    }

                }
            }

            outputJobsSmartService.Close();
            outputJobsWithoutSmartService.Close();

            // Calculate pecuniary advantage
            double averageProfitSmartService = GewinnSmartService.Average();
            double averageProfitWithoutSmartService = GewinnOhneSmartService.Average();
            double profit = averageProfitSmartService - averageProfitWithoutSmartService;

            monetaryBenefit.WriteLine("Profit average Smart Service: {0}", averageProfitSmartService);
            monetaryBenefit.WriteLine("Profit average without Smart Service: {0}", averageProfitWithoutSmartService);
            monetaryBenefit.WriteLine("Pecuniary advantage: {0}", profit);
            monetaryBenefit.Close();
            

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
        
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}
