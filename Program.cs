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
    /*
     * 
     * Diese Klasse enthält die Main-Methode.
     * Diese Klasse ist für die Simulationsdurchläufe verwantwortlich.
     * 
     */
    class Program
    {
        readonly static String[] szenario = new String[] { "alle", "Scrap_Rework", "MTBF_Downtime", "Scrap_Rework_OneYear", "Scrap_Rework_ThreeYears", "Scrap_Rework_FiveYears" };
        static int szenarioID = 5;
        private static SmartService SmartService;
        readonly static Dictionary<string, double> inputData = new Dictionary<string, double>();
        static Random random = new Random();

#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        static void Main(string[] args)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            Console.WriteLine("SzenarioID: " + szenarioID);
            // Zeit messen, wie lange das Programm braucht
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Konfigurationsfile einlesen
            // es können unterschiedliche Szenarien abgespeichert werden und oben in dem Array angegeben werden
            // mit der ID kann das gewünschte Szenario ausgewählt werden
            string curFile = @"F:\data_" + szenario[szenarioID] + ".txt";
            if (File.Exists(curFile))
            {
                Console.WriteLine("File exists.");
            }
            else
            {
                Console.WriteLine("File does not exist.");
                System.Environment.Exit(0);
            }

            // Inputdata in Dictionary<string, double> abspeichern
            foreach (string line in File.ReadAllLines(curFile, Encoding.UTF8))
            {
                string[] keyvalue = line.Split('=');
                if (keyvalue.Length == 2)
                {
                    inputData.Add(keyvalue[0], double.Parse(keyvalue[1], CultureInfo.InvariantCulture));
                }
            }

            SmartService = new SmartService(
                inputData["Scrap"],
                inputData["MTBFMean"],
                inputData["DowntimeMean"],
                inputData["DowntimeSigma"],
                inputData["Rework"]
            );

            SmartService withoutSmartService = new SmartService(
                0,
                0,
                0,
                0,
                0
            );

            StreamWriter outputSS = new StreamWriter("Output_SS_"+szenario[szenarioID]+ ".csv");
            StreamWriter outputOSS = new StreamWriter("Output_OSS_" + szenario[szenarioID] + ".csv");

            List<double> GewinnSS = new List<double>();
            List<double> GewinnOSS = new List<double>();
            List<List<Job>> saveJobsSS = new List<List<Job>>();
            List<List<Job>> saveJobsOSS = new List<List<Job>>();

            // CSV header hinzufügen
            outputSS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");
            outputOSS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");

            int i = 0;
            

            // Simulationsdurchläufe mit und ohne Smart Service
            while (i < inputData["Iterations"])
            {
                (List<Job>, List<Job>) tupelListJobs = Jobgenerator.GenerateJobs(inputData);
                

                int seed = random.Next();

                // Simulationsdurchlauf mit Smart Service
                ProcessControl pcSS = new ProcessControl(tupelListJobs.Item1, SmartService, inputData, new Simulation(randomSeed: seed));

                (Dictionary<string, double>, double, List<Job>) tupelKPIProfitSS = pcSS.Simulate();
                saveJobsSS.Add(tupelKPIProfitSS.Item3);

                // Gewinn für den geldwerten Vorteil speichern, damit der Durchschnitt errechnet werden kann
                GewinnSS.Add(tupelKPIProfitSS.Item2);

                // KPIs und den Gewinn in CSV schreiben
                foreach (var pair in tupelKPIProfitSS.Item1)
                {
                    outputSS.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost"){
                        outputSS.Write("{0}\n", tupelKPIProfitSS.Item2);
                    }
                }

                // Simulationsdurchlauf ohne Smart Service
                ProcessControl pcOSS = new ProcessControl(tupelListJobs.Item2, withoutSmartService, inputData, new Simulation(randomSeed: seed));

                (Dictionary<string, double>, double, List<Job>) tupelKPIProfitOSS = pcOSS.Simulate();
                saveJobsOSS.Add(tupelKPIProfitOSS.Item3);

                // Gewinn für den geldwerten Vorteil speichern, damit der Durchschnitt errechnet werden kann
                GewinnOSS.Add(tupelKPIProfitOSS.Item2);

                //swGOSS.WriteLine(tupelKPIProfit.Item2);
                foreach (var pair in tupelKPIProfitOSS.Item1)
                {
                    outputOSS.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost")
                    {
                        outputOSS.Write("{0}\n", tupelKPIProfitOSS.Item2);
                    }
                }

                if (i == inputData["Iterations"]/4) Console.WriteLine("Quater.");
                if (i == inputData["Iterations"]/2) Console.WriteLine("Half time.");
                if (i == (inputData["Iterations"]*3)/4) Console.WriteLine("Three quarters.");

                i++;
            }

            outputSS.Close();
            outputOSS.Close();

            StreamWriter monetaryBenefit = new StreamWriter("GeldwerterVorteil_" + szenario[szenarioID] + ".csv");
            StreamWriter outputJobsSS = new StreamWriter("Output_Jobs_SS_" + szenario[szenarioID] + ".csv");
            StreamWriter outputJobsOSS = new StreamWriter("Output_Jobs_OSS_" + szenario[szenarioID] + ".csv");

            outputJobsSS.WriteLine("Job ID; Producttype ID; Quantity");
            outputJobsOSS.WriteLine("Job ID; Producttype ID; Quantity");

            // Abgeschlossene Aufträge mit Smart Service in CSV schreiben
            foreach (List<Job> list in saveJobsSS)
            {
                foreach (Job job in list)
                {
                    outputJobsSS.Write(job.ID + ";");

                    foreach (Producttype type in job.Producttype)
                    {
                        outputJobsSS.Write(type.ID + ";" + type.Quantity + "\n");
                    }

                }
            }

            // Abgeschlossene Aufträge ohne Smart Service in CSV schreiben
            foreach (List<Job> list in saveJobsOSS)
            {
                foreach (Job job in list)
                {
                    outputJobsOSS.Write(job.ID + ";");

                    foreach (Producttype type in job.Producttype)
                    {
                        outputJobsOSS.Write(type.ID + ";" + type.Quantity + "\n");

                    }

                }
            }

            outputJobsSS.Close();
            outputJobsOSS.Close();

            // geldwerter Vorteil berechnen
            double averageSS = GewinnSS.Average();
            double averageOSS = GewinnOSS.Average();
            double profit = averageSS - averageOSS;

            monetaryBenefit.WriteLine("Gewinn Average SS: {0}", averageSS);
            monetaryBenefit.WriteLine("Gewinn Average OSS: {0}", averageOSS);
            monetaryBenefit.WriteLine("Geldwerter Vorteil: {0}", profit);
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
