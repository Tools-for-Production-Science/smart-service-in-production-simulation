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

            // Durchläufe mit SS
            int seed = 0;
            StreamWriter swKPISS = new StreamWriter("KPISS_"+szenario[szenarioID]+".csv");
            List<Double> GewinnSS = new List<Double>();
            List<Double> GewinnOSS = new List<Double>();

            // CSV header hinzufügen
            swKPISS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");
            
            int i = 0;
            while (i < inputData["Rounds"])
            {
                ProcessControl pc = new ProcessControl(Jobgenerator(), SmartService, inputData, new Simulation(randomSeed: seed));

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
                Console.WriteLine(i);

                i++;
                seed += 33;
            }

            swKPISS.Close();
            
            // Durchläufe ohne SS
            SmartService = new SmartService(
                0,
                0,
                0,
                0,
                0
            );
           
            StreamWriter swKPIOSS = new StreamWriter("KPIOSS" + szenario[szenarioID] + ".csv");
            seed = 0;    
            i = 0;
            env = new Simulation(randomSeed: 42);

            // CSV header setzten
            swKPIOSS.Write("AvailabilityPre;AvailabilityMain;AvailabilityPost;EffectivenessPre;EffectivenessMain;EffectivenessPost;ThrouputratePre;ThrouputrateMain;ThrouputratePost;ScrapRatioPre;ScrapRatioMain;ScrapRatioPost;ReworkRatioPre;ReworkRatioMain;ReworkRatioPost;NAMain;NAPost;NAPre;QBRPre;QBRMain;QBRPost;MTBFPre;MTTRPre;MTBFMain;MTTRMain;MTBFPost;MTTRPost;OEEPre;OEEMain;OEEPost;Gewinn\n");

            while (i < inputData["Rounds"])
            {
                //randomSeed: 42
                ProcessControl pc = new ProcessControl(Jobgenerator(), SmartService, inputData, new Simulation(randomSeed: seed));
                
                (Dictionary<string, double>, double) tupelKPIProfit = pc.Simulate();

                // Gewinn für den geldwerten Vorteil speichern, damit der Durchschnitt errechnet werden kann
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

        

        public static List<Job> CopyJobs(List<Job> oldJobs)
        {
            List<Job> Jobs = new List<Job>();
            for (int i = 0; i < oldJobs.Count; i++)
            {
                List<Producttype> Positions = new List<Producttype>();
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

                    Positions.Add(new Producttype(
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
