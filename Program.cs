using System;
using System.Collections.Generic;

namespace ProduktionssystemSimulation
{
    class Program
    {
        private static List<Position> Positions = new List<Position>();
        private static List<Product> Products = new List<Product>();
        private static List<Job> Jobs = new List<Job>();
        private static SmartService SmartService;

        static void Main(string[] args)
        {
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
                Products.Add(new Product( i + 1));
            }
            for (int i = 0; i < 5; i++)
            {
                Positions.Add(new Position(20, TimeSpan.FromMinutes(10), i + 1, Products,TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10)));
            }
            for (int i = 0; i < 50; i++)
            {
                Jobs.Add(new Job(i, i + 1, Positions));
            }
        }

        //public List<Position> PositionenGenerator(int positionen)
        //{

        //}

        //public List<Produkt> ProdukteGenerator()
        //{

        //}
    }
}
