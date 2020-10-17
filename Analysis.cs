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
        private TimeSpan _JobExecutionTime;
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
        static ArrayList _FinishedJobs = new ArrayList();
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
        static Dictionary<string, double> InputData = new Dictionary<string, double>();

        public Analysis(Dictionary<string,double> inputData)
        {
            InputData = inputData;
        }

        public TimeSpan AUSTPre { get => _AUSTPre; set => _AUSTPre = value; }
        public int QuantityOfReworkPre { get => _QuantityOfReworkPre; set => _QuantityOfReworkPre = value; }
        public int QuantityOfScrapPre { get => _QuantityOfScrapPre; set => _QuantityOfScrapPre = value; }
        public TimeSpan JobExecutionTime { get => _JobExecutionTime; set => _JobExecutionTime = value; }
        public ArrayList FinishedJobs { get => _FinishedJobs; set => _FinishedJobs = value; }
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

        public Dictionary<string, double> CalculateKPIs(Analysis analysis)
        {
            Dictionary<string, double> KPIs = new Dictionary<string, double>();
            KPIs.Add("AvailabilityPre", (TimeSpan.FromHours(InputData["WorkingHours"]) - analysis.AUSTPre - analysis.ADOTPre)/ TimeSpan.FromHours(InputData["WorkingHours"]));
            KPIs.Add("AvailabilityMain", (TimeSpan.FromHours(InputData["WorkingHours"]) - analysis.AUSTMain - analysis.ADOTMain) / TimeSpan.FromHours(InputData["WorkingHours"]));
            KPIs.Add("AvailabilityPost", (TimeSpan.FromHours(InputData["WorkingHours"]) - analysis.AUSTPost - analysis.ADOTPost) / TimeSpan.FromHours(InputData["WorkingHours"]));
            KPIs.Add("EffectivenessPre", (analysis.APTPre) / (TimeSpan.FromHours(InputData["WorkingHours"]) - analysis.AUSTPre - analysis.ADOTPre));
            KPIs.Add("EffectivenessMain", (analysis.APTMain) / (TimeSpan.FromHours(InputData["WorkingHours"]) - analysis.AUSTMain - analysis.ADOTMain));
            KPIs.Add("EffectivenessPost", (analysis.APTPost) / (TimeSpan.FromHours(InputData["WorkingHours"]) - analysis.AUSTPost - analysis.ADOTPost));
            KPIs.Add("ThrouputratePre", (analysis.QuantityOfGoodPre + analysis.QuantityOfReworkPre) / InputData["WorkingHours"]);
            KPIs.Add("ThrouputrateMain", (analysis.QuantityOfGoodMain + analysis.QuantityOfReworkMain) / InputData["WorkingHours"]);
            KPIs.Add("ThrouputratePost", (analysis.QuantityOfGoodPost + analysis.QuantityOfReworkPost) / InputData["WorkingHours"]);
            KPIs.Add("ScrapRatioPre", (analysis.QuantityOfScrapPre) / (analysis.QuantityOfGoodPre + analysis.QuantityOfReworkPre + analysis.QuantityOfScrapPre));
            KPIs.Add("ScrapRatioMain", (analysis.QuantityOfScrapMain) / (analysis.QuantityOfGoodMain+ analysis.QuantityOfReworkMain + analysis.QuantityOfScrapMain));
            KPIs.Add("ScrapRatioPost", (analysis.QuantityOfScrapPost) / (analysis.QuantityOfGoodPost + analysis.QuantityOfReworkPost + analysis.QuantityOfScrapPost));
            KPIs.Add("ReworkRatioPre", (analysis.QuantityOfReworkPre) / (analysis.QuantityOfGoodPre + analysis.QuantityOfReworkPre + analysis.QuantityOfScrapPre));
            KPIs.Add("ReworkRatioMain", (analysis.QuantityOfReworkMain) / (analysis.QuantityOfGoodMain + analysis.QuantityOfReworkMain + analysis.QuantityOfScrapMain));
            KPIs.Add("ReworkRatioPost", (analysis.QuantityOfReworkPost) / (analysis.QuantityOfGoodPost + analysis.QuantityOfReworkPost + analysis.QuantityOfScrapPost));
            
            try {KPIs.Add("NAMain", (analysis.QuantityOfReworkMain) / (analysis.QuantityOfReworkMain + analysis.QuantityOfScrapMain));} 
            catch(DivideByZeroException) {KPIs.Add("NAMain", 0);}
            try{KPIs.Add("NAPost", (analysis.QuantityOfReworkPost) / (analysis.QuantityOfReworkPost + analysis.QuantityOfScrapPost));}
            catch (DivideByZeroException){KPIs.Add("NAPost", 0);}
            try { KPIs.Add("NAPre", (analysis.QuantityOfReworkPre) / (analysis.QuantityOfReworkPre + analysis.QuantityOfScrapPre));}
            catch (DivideByZeroException){KPIs.Add("NAPre", 0);}
            KPIs.Add("QBRPre", (analysis.QuantityOfReworkPre + analysis.QuantityOfGoodPre) / (analysis.QuantityOfGoodPre + analysis.QuantityOfReworkPre + analysis.QuantityOfScrapPre));
            KPIs.Add("QBRMain", (analysis.QuantityOfReworkMain + analysis.QuantityOfGoodMain) / (analysis.QuantityOfGoodMain + analysis.QuantityOfReworkMain + analysis.QuantityOfScrapMain));
            KPIs.Add("QBRPost", (analysis.QuantityOfReworkPost + analysis.QuantityOfGoodPost) / (analysis.QuantityOfGoodPost + analysis.QuantityOfReworkPost + analysis.QuantityOfScrapPost));
            try{KPIs.Add("MTBFPre", InputData["WorkingHours"] / analysis.NumberOfFailurePre); }
            catch (DivideByZeroException) { KPIs.Add("MTBFPre", 0); }
            try{KPIs.Add("MTBFMain", InputData["WorkingHours"] / analysis.NumberOfFailureMain);}
            catch (DivideByZeroException) { KPIs.Add("MTBFMain", 0); }
            try{KPIs.Add("MTBFPost", InputData["WorkingHours"] / analysis.NumberOfFailurePost);}
            catch (DivideByZeroException) { KPIs.Add("MTBFPost", 0); }
            try{KPIs.Add("MTTRPre", analysis.ADOTPre.TotalMinutes / analysis.NumberOfFailurePre);}
            catch (DivideByZeroException) { KPIs.Add("MTTRPre", 0); }
            try{ KPIs.Add("MTTRMain", analysis.ADOTMain.TotalMinutes / analysis.NumberOfFailureMain);}
            catch (DivideByZeroException) { KPIs.Add("MTTRMain", 0); }
            try{ KPIs.Add("MTTRPost", analysis.ADOTPost.TotalMinutes / analysis.NumberOfFailurePost);}
            catch (DivideByZeroException) { KPIs.Add("MTTRPost", 0); }
            KPIs.Add("OEEPre", KPIs["AvailabilityPre"] * KPIs["EffectivenessPre"] * KPIs["QBRPre"]);
            KPIs.Add("OEEMain", KPIs["AvailabilityMain"] * KPIs["EffectivenessMain"] * KPIs["QBRMain"]);
            KPIs.Add("OEEPost", KPIs["AvailabilityPost"] * KPIs["EffectivenessPost"] * KPIs["QBRPost"]);
            return KPIs;
        }

        public double Profit(Analysis analysis)
        {
            
            ManufactoringCosts = (InputData["WorkingHours"]-((analysis.ADOTPre.TotalMinutes+ analysis.ADOTMain.TotalMinutes+ analysis.ADOTPost.TotalMinutes) /60))*InputData["MachineHourCosts"]; 
            foreach (Job j in FinishedJobs)
            {
                foreach (Position p in j.Positions)
                {
                    Revenue += (p.Quantity * p.Price);
                    MaterialCosts += (p.TotalProducedQuantity * p.MaterialCost);
                    foreach (Product pr in p.Products)
                    {
                        LabourCosts += (pr.TotalReworkTime.Minutes /60) * InputData["HourlyWage"];
                    }
                }
            }
            RepairCosts = ((analysis.ADOTPre + analysis.ADOTMain + analysis.ADOTPost).TotalMinutes / 60) * InputData["HourlyWageFitter"];
            Costs = ManufactoringCosts + MaterialCosts + LabourCosts + RepairCosts;
            return Revenue-Costs;
        }
    }
}
