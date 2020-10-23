using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Program
    {
        readonly static String[] szenario = new String[] { "alle", "Scrap_Rework", "MTBF_Downtime" };
        readonly static int szenarioID = 2;
        private static SmartService SmartService;
        readonly static Dictionary<string, double> inputData = new Dictionary<string, double>();
        // Simulationsumgegbung wird hier schon erzeugt, da diese für die festlegung der Produktmenge benötigt wird
        private static Simulation env = new Simulation(randomSeed: 42);

#pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
        static void Main(string[] args)
#pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen
        {
            // Zeit messen, wie lange das Programm braucht
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Konfigurationsfile einlesen
            // es können unterschiedliche Szenarien abgespeichert werden und oben in dem Array angegeben werden
            // mit der ID kann das gewünschte Szenario ausgewählt werden
            //string curFile = @"C:\Users\kfausel\Documents\Simulation_BA\data_" + szenario[szenarioID] + ".txt";
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

            SmartService OhneSmartService = new SmartService(
                0,
                0,
                0,
                0,
                0
            );

            // Durchläufe mit SS
            StreamWriter swKPISS = new StreamWriter("KPISS_"+szenario[szenarioID]+".csv");
            StreamWriter swKPIOSS = new StreamWriter("KPIOSS" + szenario[szenarioID] + ".csv");
            List<Double> GewinnSS = new List<Double>();
            List<Double> GewinnOSS = new List<Double>();

            // CSV header hinzufügen
            swKPISS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");
            swKPIOSS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");

            int i = 0;
            int seed = 0;

            while (i < inputData["Iterations"])
            {
                (List<Job>, List<Job>) tupelListJobs = Jobgenerator.JobgeneratorS(inputData, seed+827);

                //SS
                ProcessControl pc = new ProcessControl(tupelListJobs.Item1, SmartService, inputData, new Simulation(randomSeed: seed));

                (Dictionary<string, double>, double) tupelKPIProfit = pc.Simulate();

                // Gewinn für den geldwerten Vorteil speichern, damit der Durchschnitt errechnet werden kann
                GewinnSS.Add(tupelKPIProfit.Item2);

                // KPIs und den Gewinn in CSV schreiben
                foreach (var pair in tupelKPIProfit.Item1)
                {
                    swKPISS.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost"){
                        swKPISS.Write("{0}\n", tupelKPIProfit.Item2);
                    }
                }
                Console.WriteLine(seed);
                // OSS
                ProcessControl pc2 = new ProcessControl(tupelListJobs.Item2, OhneSmartService, inputData, new Simulation(randomSeed: seed));

                (Dictionary<string, double>, double) tupelKPIProfit2 = pc2.Simulate();

                // Gewinn für den geldwerten Vorteil speichern, damit der Durchschnitt errechnet werden kann
                GewinnOSS.Add(tupelKPIProfit2.Item2);

                //swGOSS.WriteLine(tupelKPIProfit.Item2);
                foreach (var pair in tupelKPIProfit2.Item1)
                {
                    swKPIOSS.Write("{0};", pair.Value);
                    if (pair.Key == "OEEPost")
                    {
                        swKPIOSS.Write("{0}\n", tupelKPIProfit2.Item2);
                    }
                }

                i++;
                seed += 345;
            }

            swKPISS.Close();
            swKPIOSS.Close();

            StreamWriter Vorteil = new StreamWriter("GeldwerterVorteil" + szenario[szenarioID] + ".csv");

            double averageSS = GewinnSS.Average();
            double averageOSS = GewinnOSS.Average();
            double profit = averageSS - averageOSS;

            Vorteil.WriteLine("Gewinn Average SS: {0}", averageSS);
            Vorteil.WriteLine("Gewinn Average OSS: {0}", averageOSS);
            Vorteil.WriteLine("Geldwerter Vorteil: {0}", profit);
            Vorteil.Close();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
        
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}
