using System;
using System.Collections;
using System.Collections.Generic;


namespace ProduktionssystemSimulation
{
    public class Analysis
    {
        private TimeSpan _AUSTPre = TimeSpan.FromDays(0);
        private TimeSpan _AUSTMain = TimeSpan.FromDays(0);
        private TimeSpan _AUSTPost = TimeSpan.FromDays(0);
        private double _QuantityOfReworkPre;
        private double _QuantityOfScrapPre;
        private double _QuantityOfReworkMain;
        private double _QuantityOfScrapMain;
        private double _QuantityOfReworkPost;
        private double _QuantityOfGoodPre;
        private double _QuantityOfGoodMain;
        private double _QuantityOfGoodPost;
        private double _QuantityOfScrapPost;
        private double _Costs = 0;
        private double _NumberOfFailurePre;
        private double _NumberOfFailureMain;
        private double _NumberOfFailurePost;
        private ArrayList _FinishedJobs = new ArrayList();
        private TimeSpan _MaschineAnPre = TimeSpan.FromDays(0);
        private TimeSpan _MaschineAnMain = TimeSpan.FromDays(0);
        private TimeSpan _MaschineAnPost = TimeSpan.FromDays(0);
        private TimeSpan _DowntimePre = TimeSpan.FromDays(0);
        private TimeSpan _ADOTMain = TimeSpan.FromDays(0);
        private TimeSpan _ADOTPost = TimeSpan.FromDays(0);
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
        public double QuantityOfReworkPre { get => _QuantityOfReworkPre; set => _QuantityOfReworkPre = value; }
        public double QuantityOfScrapPre { get => _QuantityOfScrapPre; set => _QuantityOfScrapPre = value; }
        public TimeSpan APTPre { get => _MaschineAnPre; set => _MaschineAnPre = value; }
        public TimeSpan APTMain { get => _MaschineAnMain; set => _MaschineAnMain = value; }
        public TimeSpan APTPost { get => _MaschineAnPost; set => _MaschineAnPost = value; }
        public TimeSpan ADOTPre { get => _DowntimePre; set => _DowntimePre = value; }
        public TimeSpan ADOTMain { get => _ADOTMain; set => _ADOTMain = value; }
        public TimeSpan ADOTPost { get => _ADOTPost; set => _ADOTPost = value; }
        public double QuantityOfReworkMain { get => _QuantityOfReworkMain; set => _QuantityOfReworkMain = value; }
        public double QuantityOfScrapMain { get => _QuantityOfScrapMain; set => _QuantityOfScrapMain = value; }
        public double QuantityOfReworkPost { get => _QuantityOfReworkPost; set => _QuantityOfReworkPost = value; }
        public double QuantityOfScrapPost { get => _QuantityOfScrapPost; set => _QuantityOfScrapPost = value; }
        public TimeSpan AUSTMain { get => _AUSTMain; set => _AUSTMain = value; }
        public TimeSpan AUSTPost { get => _AUSTPost; set => _AUSTPost = value; }
        public double QuantityOfGoodPre { get => _QuantityOfGoodPre; set => _QuantityOfGoodPre = value; }
        public double QuantityOfGoodMain { get => _QuantityOfGoodMain; set => _QuantityOfGoodMain = value; }
        public double QuantityOfGoodPost { get => _QuantityOfGoodPost; set => _QuantityOfGoodPost = value; }
        public double NumberOfFailurePre { get => _NumberOfFailurePre; set => _NumberOfFailurePre = value; }
        public double NumberOfFailureMain { get => _NumberOfFailureMain; set => _NumberOfFailureMain = value; }
        public double NumberOfFailurePost { get => _NumberOfFailurePost; set => _NumberOfFailurePost = value; }
        public double Costs { get => _Costs; set => _Costs = value; }
        public Dictionary<string, double> InputData { get => _InputData; set => _InputData = value; }
        public ArrayList FinishedJobs { get => _FinishedJobs; set => _FinishedJobs = value; }

        public Dictionary<string, double> CalculateKPIs()
        {
            Dictionary<string, double> KPIs = new Dictionary<string, double>();
            double WorkingMinutes = InputData["WorkingHours"] * 60;
            KPIs.Add("AvailabilityPre", (WorkingMinutes - AUSTPre.TotalMinutes - ADOTPre.TotalMinutes) / WorkingMinutes);
            KPIs.Add("AvailabilityMain", (WorkingMinutes - AUSTMain.TotalMinutes - ADOTMain.TotalMinutes) / WorkingMinutes);
            KPIs.Add("AvailabilityPost", (WorkingMinutes - AUSTPost.TotalMinutes - ADOTPost.TotalMinutes) / WorkingMinutes);
            KPIs.Add("EffectivenessPre", (APTPre.TotalMinutes) / (InputData["CapacityPre"]*WorkingMinutes - AUSTPre.TotalMinutes - ADOTPre.TotalMinutes));
            KPIs.Add("EffectivenessMain", (APTMain.TotalMinutes) / (WorkingMinutes - AUSTMain.TotalMinutes - ADOTMain.TotalMinutes));
            KPIs.Add("EffectivenessPost", (APTPost.TotalMinutes) / (WorkingMinutes - AUSTPost.TotalMinutes - ADOTPost.TotalMinutes));
            KPIs.Add("ThrouputratePre", (QuantityOfGoodPre + QuantityOfReworkPre) / InputData["WorkingHours"]);
            KPIs.Add("ThrouputrateMain", (QuantityOfGoodMain + QuantityOfReworkMain) / InputData["WorkingHours"]);
            KPIs.Add("ThrouputratePost", (QuantityOfGoodPost + QuantityOfReworkPost) / InputData["WorkingHours"]);
            KPIs.Add("ScrapRatioPre", (QuantityOfScrapPre / (QuantityOfGoodPre + QuantityOfReworkPre + QuantityOfScrapPre)));
            KPIs.Add("ScrapRatioMain", (QuantityOfScrapMain / (QuantityOfGoodMain+ QuantityOfReworkMain + QuantityOfScrapMain)));
            KPIs.Add("ScrapRatioPost", (QuantityOfScrapPost / (QuantityOfGoodPost + QuantityOfReworkPost + QuantityOfScrapPost)));
            KPIs.Add("ReworkRatioPre", (QuantityOfReworkPre / (QuantityOfGoodPre + QuantityOfReworkPre + QuantityOfScrapPre)));
            KPIs.Add("ReworkRatioMain", (QuantityOfReworkMain / (QuantityOfGoodMain + QuantityOfReworkMain + QuantityOfScrapMain)));
            KPIs.Add("ReworkRatioPost", (QuantityOfReworkPost / (QuantityOfGoodPost + QuantityOfReworkPost + QuantityOfScrapPost)));
            if((QuantityOfReworkMain + QuantityOfScrapMain) != 0)
            {
                KPIs.Add("NAMain", QuantityOfReworkMain / (QuantityOfReworkMain + QuantityOfScrapMain));
            }else
            {
                KPIs.Add("NAMain", 0);
            }
            if ((QuantityOfReworkPost + QuantityOfScrapPost) != 0)
            {
                KPIs.Add("NAPost", (QuantityOfReworkPost / (QuantityOfReworkPost + QuantityOfScrapPost)));
            }else
            {
                KPIs.Add("NAPost", 0);
            }
            if ((QuantityOfReworkPre + QuantityOfScrapPre) !=0)
            {
                KPIs.Add("NAPre", (QuantityOfReworkPre / (QuantityOfReworkPre + QuantityOfScrapPre)));
            }else
            {
                KPIs.Add("NAPre", 0);
            }
            KPIs.Add("QBRPre", ((QuantityOfReworkPre + QuantityOfGoodPre) / (QuantityOfGoodPre + QuantityOfReworkPre + QuantityOfScrapPre)));
            KPIs.Add("QBRMain", ((QuantityOfReworkMain + QuantityOfGoodMain) / (QuantityOfGoodMain + QuantityOfReworkMain + QuantityOfScrapMain)));
            KPIs.Add("QBRPost", ((QuantityOfReworkPost + QuantityOfGoodPost) / (QuantityOfGoodPost + QuantityOfReworkPost + QuantityOfScrapPost)));
            if (NumberOfFailurePre != 0)
            {
                KPIs.Add("MTBFPre", InputData["WorkingHours"] / NumberOfFailurePre);
                KPIs.Add("MTTRPre", ADOTPre.TotalHours / NumberOfFailurePre);
            }else
            {
                KPIs.Add("MTBFPre", 0);
                KPIs.Add("MTTRPre", 0);
            }
            if (NumberOfFailureMain != 0)
            {
                KPIs.Add("MTBFMain", InputData["WorkingHours"] / NumberOfFailureMain);
                KPIs.Add("MTTRMain", ADOTMain.TotalHours / NumberOfFailureMain);
            }
            else
            {
                KPIs.Add("MTBFMain", 0);
                KPIs.Add("MTTRMain", 0);
            }
            if (NumberOfFailurePost!=0)
            {
                KPIs.Add("MTBFPost", InputData["WorkingHours"] / NumberOfFailurePost);
                KPIs.Add("MTTRPost", ADOTPost.TotalHours / NumberOfFailurePost);
            }
            else
            {
                KPIs.Add("MTBFPost", 0);
                KPIs.Add("MTTRPost", 0);
            }
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
                    //Console.WriteLine("Menge: " +p.Quantity);
                    //Console.WriteLine("Totale Menge: " + p.TotalProducedQuantity);
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
            //Console.WriteLine("Umsatz: " + Revenue);
            //Console.WriteLine("Kosten: " + Costs);
            //Console.WriteLine("ManufactoringCosts: " + ManufactoringCosts);
            //Console.WriteLine("MaterialCosts: " + MaterialCosts);
            //Console.WriteLine("RepairCosts: " + RepairCosts);
            //Console.WriteLine("LabourCosts: " + LabourCosts);
            return Revenue-Costs;
        }
    }
}
