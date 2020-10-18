using System;
using System.Collections;
using System.Collections.Generic;


namespace ProduktionssystemSimulation
{
    public class Analysis
    {
        private TimeSpan _AUSTPre;
        private TimeSpan _AUSTMain;
        private TimeSpan _AUSTPost;
        private int _QuantityOfReworkPre;
        private int _QuantityOfScrapPre;
        private int _QuantityOfReworkMain;
        private int _QuantityOfScrapMain;
        private int _QuantityOfReworkPost;
        private int _QuantityOfGoodPre;
        private int _QuantityOfGoodMain;
        private int _QuantityOfGoodPost;
        private int _QuantityOfScrapPost;
        private double _Costs = 0;
        private int _NumberOfFailurePre;
        private int _NumberOfFailureMain;
        private int _NumberOfFailurePost;
        private ArrayList _FinishedJobs = new ArrayList();
        private static TimeSpan _MaschineAnPre = TimeSpan.FromDays(0);
        private static TimeSpan _MaschineAnMain = TimeSpan.FromDays(0);
        private static TimeSpan _MaschineAnPost = TimeSpan.FromDays(0);
        private static TimeSpan _DowntimePre = TimeSpan.FromDays(0);
        private static TimeSpan _ADOTMain = TimeSpan.FromDays(0);
        private static TimeSpan _ADOTPost = TimeSpan.FromDays(0);
        double Revenue = 0;
        double LabourCosts = 0;
        double MaterialCosts = 0;
        double ManufactoringCosts = 0;
        double RepairCosts = 0;
        private Dictionary<string, double> _InputData = new Dictionary<string, double>();

        public Analysis(Dictionary<string,double> inputData)
        {
            InputData = inputData;
        }

        public TimeSpan AUSTPre { get => _AUSTPre; set => _AUSTPre = value; }
        public int QuantityOfReworkPre { get => _QuantityOfReworkPre; set => _QuantityOfReworkPre = value; }
        public int QuantityOfScrapPre { get => _QuantityOfScrapPre; set => _QuantityOfScrapPre = value; }
        public TimeSpan APTPre { get => _MaschineAnPre; set => _MaschineAnPre = value; }
        public TimeSpan APTMain { get => _MaschineAnMain; set => _MaschineAnMain = value; }
        public TimeSpan APTPost { get => _MaschineAnPost; set => _MaschineAnPost = value; }
        public TimeSpan ADOTPre { get => _DowntimePre; set => _DowntimePre = value; }
        public TimeSpan ADOTMain { get => _ADOTMain; set => _ADOTMain = value; }
        public TimeSpan ADOTPost { get => _ADOTPost; set => _ADOTPost = value; }
        public int QuantityOfReworkMain { get => _QuantityOfReworkMain; set => _QuantityOfReworkMain = value; }
        public int QuantityOfScrapMain { get => _QuantityOfScrapMain; set => _QuantityOfScrapMain = value; }
        public int QuantityOfReworkPost { get => _QuantityOfReworkPost; set => _QuantityOfReworkPost = value; }
        public int QuantityOfScrapPost { get => _QuantityOfScrapPost; set => _QuantityOfScrapPost = value; }
        public TimeSpan AUSTMain { get => _AUSTMain; set => _AUSTMain = value; }
        public TimeSpan AUSTPost { get => _AUSTPost; set => _AUSTPost = value; }
        public int QuantityOfGoodPre { get => _QuantityOfGoodPre; set => _QuantityOfGoodPre = value; }
        public int QuantityOfGoodMain { get => _QuantityOfGoodMain; set => _QuantityOfGoodMain = value; }
        public int QuantityOfGoodPost { get => _QuantityOfGoodPost; set => _QuantityOfGoodPost = value; }
        public int NumberOfFailurePre { get => _NumberOfFailurePre; set => _NumberOfFailurePre = value; }
        public int NumberOfFailureMain { get => _NumberOfFailureMain; set => _NumberOfFailureMain = value; }
        public int NumberOfFailurePost { get => _NumberOfFailurePost; set => _NumberOfFailurePost = value; }
        public double Costs { get => _Costs; set => _Costs = value; }
        public Dictionary<string, double> InputData { get => _InputData; set => _InputData = value; }
        public ArrayList FinishedJobs { get => _FinishedJobs; set => _FinishedJobs = value; }

        public Dictionary<string, double> CalculateKPIs()
        {
            Dictionary<string, double> KPIs = new Dictionary<string, double>();
            KPIs.Add("AvailabilityPre", (TimeSpan.FromHours(InputData["WorkingHours"]) - AUSTPre - ADOTPre)/ TimeSpan.FromHours(InputData["WorkingHours"]));
            KPIs.Add("AvailabilityMain", (TimeSpan.FromHours(InputData["WorkingHours"]) - AUSTMain - ADOTMain) / TimeSpan.FromHours(InputData["WorkingHours"]));
            KPIs.Add("AvailabilityPost", (TimeSpan.FromHours(InputData["WorkingHours"]) - AUSTPost - ADOTPost) / TimeSpan.FromHours(InputData["WorkingHours"]));
            KPIs.Add("EffectivenessPre", (APTPre) / (TimeSpan.FromHours(InputData["WorkingHours"]) - AUSTPre - ADOTPre));
            KPIs.Add("EffectivenessMain", (APTMain) / (TimeSpan.FromHours(InputData["WorkingHours"]) - AUSTMain - ADOTMain));
            KPIs.Add("EffectivenessPost", (APTPost) / (TimeSpan.FromHours(InputData["WorkingHours"]) - AUSTPost - ADOTPost));
            KPIs.Add("ThrouputratePre", (QuantityOfGoodPre + QuantityOfReworkPre) / InputData["WorkingHours"]);
            KPIs.Add("ThrouputrateMain", (QuantityOfGoodMain + QuantityOfReworkMain) / InputData["WorkingHours"]);
            KPIs.Add("ThrouputratePost", (QuantityOfGoodPost + QuantityOfReworkPost) / InputData["WorkingHours"]);
            KPIs.Add("ScrapRatioPre", (QuantityOfScrapPre) / (QuantityOfGoodPre + QuantityOfReworkPre + QuantityOfScrapPre));
            KPIs.Add("ScrapRatioMain", (QuantityOfScrapMain) / (QuantityOfGoodMain+ QuantityOfReworkMain + QuantityOfScrapMain));
            KPIs.Add("ScrapRatioPost", (QuantityOfScrapPost) / (QuantityOfGoodPost + QuantityOfReworkPost + QuantityOfScrapPost));
            KPIs.Add("ReworkRatioPre", (QuantityOfReworkPre) / (QuantityOfGoodPre + QuantityOfReworkPre + QuantityOfScrapPre));
            KPIs.Add("ReworkRatioMain", (QuantityOfReworkMain) / (QuantityOfGoodMain + QuantityOfReworkMain + QuantityOfScrapMain));
            KPIs.Add("ReworkRatioPost", (QuantityOfReworkPost) / (QuantityOfGoodPost + QuantityOfReworkPost + QuantityOfScrapPost));
            
            try {KPIs.Add("NAMain", (QuantityOfReworkMain) / (QuantityOfReworkMain + QuantityOfScrapMain));} 
            catch(DivideByZeroException) {KPIs.Add("NAMain", 0);}
            try{KPIs.Add("NAPost", (QuantityOfReworkPost) / (QuantityOfReworkPost + QuantityOfScrapPost));}
            catch (DivideByZeroException){KPIs.Add("NAPost", 0);}
            try { KPIs.Add("NAPre", (QuantityOfReworkPre) / (QuantityOfReworkPre + QuantityOfScrapPre));}
            catch (DivideByZeroException){KPIs.Add("NAPre", 0);}
            KPIs.Add("QBRPre", (QuantityOfReworkPre + QuantityOfGoodPre) / (QuantityOfGoodPre + QuantityOfReworkPre + QuantityOfScrapPre));
            KPIs.Add("QBRMain", (QuantityOfReworkMain + QuantityOfGoodMain) / (QuantityOfGoodMain + QuantityOfReworkMain + QuantityOfScrapMain));
            KPIs.Add("QBRPost", (QuantityOfReworkPost + QuantityOfGoodPost) / (QuantityOfGoodPost + QuantityOfReworkPost + QuantityOfScrapPost));
            try{KPIs.Add("MTBFPre", InputData["WorkingHours"] / NumberOfFailurePre); }
            catch (DivideByZeroException) { KPIs.Add("MTBFPre", 0); }
            try{KPIs.Add("MTBFMain", InputData["WorkingHours"] / NumberOfFailureMain);}
            catch (DivideByZeroException) { KPIs.Add("MTBFMain", 0); }
            try{KPIs.Add("MTBFPost", InputData["WorkingHours"] / NumberOfFailurePost);}
            catch (DivideByZeroException) { KPIs.Add("MTBFPost", 0); }
            try{KPIs.Add("MTTRPre", ADOTPre.TotalMinutes / NumberOfFailurePre);}
            catch (DivideByZeroException) { KPIs.Add("MTTRPre", 0); }
            try{ KPIs.Add("MTTRMain", ADOTMain.TotalMinutes / NumberOfFailureMain);}
            catch (DivideByZeroException) { KPIs.Add("MTTRMain", 0); }
            try{ KPIs.Add("MTTRPost", ADOTPost.TotalMinutes / NumberOfFailurePost);}
            catch (DivideByZeroException) { KPIs.Add("MTTRPost", 0); }
            KPIs.Add("OEEPre", KPIs["AvailabilityPre"] * KPIs["EffectivenessPre"] * KPIs["QBRPre"]);
            KPIs.Add("OEEMain", KPIs["AvailabilityMain"] * KPIs["EffectivenessMain"] * KPIs["QBRMain"]);
            KPIs.Add("OEEPost", KPIs["AvailabilityPost"] * KPIs["EffectivenessPost"] * KPIs["QBRPost"]);
            return KPIs;
        }

        public double Profit()
        {
            ManufactoringCosts = (InputData["WorkingHours"]-((ADOTPre.TotalMinutes+ ADOTMain.TotalMinutes+ ADOTPost.TotalMinutes) /60))*InputData["MachineHourCosts"]; 
            foreach (Job j in FinishedJobs)
            {
                foreach (Position p in j.Positions)
                {
                    Console.WriteLine("Menge: " +p.Quantity);
                    Console.WriteLine("Totale Menge: " + p.TotalProducedQuantity);
                    //Console.WriteLine("Materialkosten pro Stück: " +  p.MaterialCost);
                    Revenue += (p.Quantity * p.Price);
                    MaterialCosts += (p.TotalProducedQuantity * p.MaterialCost);
                    foreach (Product pr in p.Products)
                    {
                        LabourCosts += (pr.TotalReworkTime.Minutes /60) * InputData["HourlyWage"];
                    }
                }
            }
            RepairCosts = ((ADOTPre.Add(ADOTMain).Add(ADOTPost)).TotalMinutes / 60) * InputData["HourlyWageFitter"];
            Costs = ManufactoringCosts + MaterialCosts + LabourCosts + RepairCosts;
            Console.WriteLine("Umsatz: " + Revenue);
            Console.WriteLine("Kosten: " + Costs);
            Console.WriteLine("ManufactoringCosts: " + ManufactoringCosts);
            Console.WriteLine("MaterialCosts: " + MaterialCosts);
            Console.WriteLine("RepairCosts: " + RepairCosts);
            Console.WriteLine("LabourCosts: " + LabourCosts);
            return Revenue-Costs;
        }
    }
}
