using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProduktionssystemSimulation
{
    class Program
    {
        private static List<Position> Positions = new List<Position>();
        private static List<Product> Products = new List<Product>();
        private static List<Job> Jobs = new List<Job>();
        private static SmartService SmartService;
        private static double _QuantityMean;
        private static double _QuantitySigma;
        private static ArrayList _Quantity = new ArrayList();
        public static double ProductionTimePiecesPreMean;
        public static double ProductionTimePiecesMainMean;
        public static double ProductionTimePiecesPostMean;
        public static double ProductionTimePiecesPreSigma;
        public static double ProductionTimePiecesMainSigma;
        public static double ProductionTimePiecesPostSigma;
        public static double SetupPreMean;
        public static double SetupMainMean;
        public static double SetupPostMean;
        public static double SetupPreSigma;
        public static double SetupMainSigma;
        public static double SetupPostSigma;
        public static double ScrapRatioPreMean;
        public static double ScrapRatioMainMean;
        public static double ScrapRatioPostMean;
        public static double ScrapRatioPreSigma;
        public static double ScrapRatioMainSigma;
        public static double ScrapRatioPostSigma;
        public static ArrayList ProductionTimePre = new ArrayList();
        public static ArrayList ProductionTimeMain = new ArrayList();
        public static ArrayList ProductionTimePost = new ArrayList();
        public static ArrayList SetupPre = new ArrayList();
        public static ArrayList SetupMain = new ArrayList();
        public static ArrayList SetupPost = new ArrayList();
        public static ArrayList ScrapRatioPre = new ArrayList();
        public static ArrayList ScrapRatioMain = new ArrayList();
        public static ArrayList ScrapRatioPost = new ArrayList();

        public static double QuantitySigma { get => _QuantitySigma; set => _QuantitySigma = value; }
        public static double QuantityMean { get => _QuantityMean; set => _QuantityMean = value; }
        public static ArrayList Quantity { get => _Quantity; set => _Quantity = value; }

        static void Main(string[] args)
        {
            string line;
            List<string[]> liste = new List<string[]>();
            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"F:\HPO_Daten.csv");
            while ((line = file.ReadLine()) != null)
            {
                liste.Add(line.Split(';'));
            }
            file.Close();

            foreach (string[] element in liste.Skip(1))
            {
                ProductionTimePre.Add(double.Parse(element[3], System.Globalization.CultureInfo.InvariantCulture));
                ProductionTimeMain.Add(double.Parse(element[18], System.Globalization.CultureInfo.InvariantCulture));
                ProductionTimePost.Add(double.Parse(element[33], System.Globalization.CultureInfo.InvariantCulture));
                SetupPre.Add(double.Parse(element[4], System.Globalization.CultureInfo.InvariantCulture));
                SetupMain.Add(double.Parse(element[19], System.Globalization.CultureInfo.InvariantCulture));
                SetupPost.Add(double.Parse(element[34], System.Globalization.CultureInfo.InvariantCulture));
                ScrapRatioPre.Add(double.Parse(element[16], System.Globalization.CultureInfo.InvariantCulture));
                ScrapRatioMain.Add(double.Parse(element[31], System.Globalization.CultureInfo.InvariantCulture));
                ScrapRatioPost.Add(double.Parse(element[46], System.Globalization.CultureInfo.InvariantCulture));
                Quantity.Add(double.Parse(element[47], System.Globalization.CultureInfo.InvariantCulture));
            }

            ProductionTimePiecesPreMean = Mean(ProductionTimePre);
            ProductionTimePiecesMainMean = Mean(ProductionTimeMain);
            ProductionTimePiecesPostMean = Mean(ProductionTimePost);
            SetupPreMean = Mean(SetupPre);
            SetupMainMean = Mean(SetupMain);
            SetupPostMean = Mean(SetupPost);
            ScrapRatioPreMean = Mean(ScrapRatioPre);
            ScrapRatioMainMean = Mean(ScrapRatioMain);
            ScrapRatioPostMean = Mean(ScrapRatioPost);
            QuantityMean = Mean(Quantity);

            ProductionTimePiecesPreSigma = StandardDeviation(ProductionTimePre);
            ProductionTimePiecesMainSigma = StandardDeviation(ProductionTimeMain);
            ProductionTimePiecesPostSigma = StandardDeviation(ProductionTimePost);
            SetupPreSigma = StandardDeviation(SetupPre);
            SetupMainSigma = StandardDeviation(SetupMain);
            SetupPostSigma = StandardDeviation(SetupPost);
            ScrapRatioPreSigma = StandardDeviation(ScrapRatioPre);
            ScrapRatioMainSigma = StandardDeviation(ScrapRatioMain);
            ScrapRatioPostSigma = StandardDeviation(ScrapRatioPost);
            QuantitySigma = StandardDeviation(Quantity);

            Jobgenerator();

            Console.WriteLine("Do you like to activate the SS?:");
            Console.WriteLine("\ty - Yes");
            Console.WriteLine("\tn - No");

            switch (Console.ReadLine())
            {
                case "y":
                    Console.WriteLine("Smart Service is activated.");
                    SmartService = new SmartService(0.8, 600, 100, TimeSpan.FromDays(365), 2000, 1, 1);
                    Console.WriteLine("Configure  Smart Service:");
                    Console.WriteLine("Maschinen Ausfallwahrscheinlichkeit:");
                    Console.WriteLine("\ty - Yes");
                    Console.WriteLine("\tn - No");

                    switch (Console.ReadLine())
                    {
                        case "j":
                            Console.WriteLine("Maschinen Ausfallwahrscheinlichkeit wird verbessert");
                            SmartService.MaschinenAusfallwkeit = 1;
                            break;
                        case "n":
                            Console.WriteLine("Maschinen Ausfallwahrscheinlichkeit wird nicht beeinflusst");
                            SmartService.MaschinenAusfallwkeit = 0;
                            break;
                    }

                    Console.WriteLine("Menge an Ausschuss:");
                    Console.WriteLine("\ty - Yes");
                    Console.WriteLine("\tn - No");

                    switch (Console.ReadLine())
                    {
                        case "j":
                            Console.WriteLine("Menge an Ausschuss wird verbessert");
                            SmartService.Ausschuss = 1;
                            break;
                        case "n":
                            Console.WriteLine("Menge an Ausschuss wird nicht beeinflusst");
                            SmartService.Ausschuss = 0;
                            break;
                    }

                    Console.WriteLine("Maschinen Reparaturzeit:");
                    Console.WriteLine("\ty - Yes");
                    Console.WriteLine("\tn - No");

                    switch (Console.ReadLine())
                    {
                        case "j":
                            Console.WriteLine("Maschinen Reparaturzeit wird verbessert");
                            SmartService.MaschinenReparaturzeit = 1;
                            break;
                        case "n":
                            Console.WriteLine("Maschinen Reparaturzeit wird nicht beeinflusst");
                            SmartService.MaschinenReparaturzeit = 0;
                            break;
                    }


                    break;
                case "n":
                    SmartService = new SmartService(1, 0, 0, TimeSpan.FromDays(0), 0, 0, 0);
                    break;
            }
            ProcessControl pc = new ProcessControl(Jobs, SmartService);
            pc.Simulate();
        }

        public static void Jobgenerator()
        {
            for (int i = 0; i < 20; i++)
            {
                Products.Add(new Product(i + 1, TimeSpan.FromDays(ProductionTimePiecesPreMean), TimeSpan.FromDays(ProductionTimePiecesMainMean), TimeSpan.FromDays(ProductionTimePiecesPostMean), TimeSpan.FromDays(ProductionTimePiecesPreSigma), TimeSpan.FromDays(ProductionTimePiecesMainSigma), TimeSpan.FromDays(ProductionTimePiecesPostSigma), ScrapRatioPreMean, ScrapRatioPreSigma, ScrapRatioMainMean, ScrapRatioMainSigma, ScrapRatioPostMean, ScrapRatioPostSigma));
            }
            for (int i = 0; i < 1; i++)
            {
                Positions.Add(new Position(20, i + 1, Products, TimeSpan.FromDays(SetupPreMean), TimeSpan.FromDays(SetupPreSigma), TimeSpan.FromDays(SetupMainMean), TimeSpan.FromDays(SetupMainSigma), TimeSpan.FromDays(SetupPostMean), TimeSpan.FromDays(SetupPostSigma)));
            }
            for (int i = 0; i < 50; i++)
            {
                Jobs.Add(new Job(i, i + 1, Positions));
            }
            Console.WriteLine("Anzahl Jobs: " + Jobs.Count);
        }

        //public List<Position> PositionenGenerator(int positionen)
        //{

        //}

        //public List<Produkt> ProdukteGenerator()
        //{

        //}

        public static double Mean(ArrayList array)
        {
            double sum = 0;
            double result = 0;
            int i = 0;

            foreach (double element in array)
            {
                i++;
                sum += element;
            }
            result = sum / i;
            return result;
        }

        public static double StandardDeviation(ArrayList array)
        {
            double mean = Mean(array);
            double sum = 0;
            double result = 0;
            int i = 0;
            foreach (double element in array)
            {
                i++;
                sum += Math.Pow(element, 2);
            }
            double mittelwertDerQuadrate = sum / i;
            result = mittelwertDerQuadrate - Math.Pow(mean, 2);
            return Math.Sqrt(result);
        }
    }
}
