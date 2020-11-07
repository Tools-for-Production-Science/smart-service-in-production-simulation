using System;
using System.Collections;
using System.Collections.Generic;


namespace ProduktionssystemSimulation
{
    /*
     * 
     * Diese Klasse berechnet den Gewinn und die KPIs eines Simulationsduchlaufs.
     * 
     */
    public class Analysis
    {
        private TimeSpan _setupTimePre = TimeSpan.FromDays(0);
        private TimeSpan _setupTimeMain = TimeSpan.FromDays(0);
        private TimeSpan _setupTimePost = TimeSpan.FromDays(0);
        private double _QuantityOfReworkPre;
        private double _QuantityOfScrapPre;
        private double _QuantityOfReworkMain;
        private double _QuantityOfScrapMain;
        private double _QuantityOfReworkPost;
        private double _QuantityOfGoodPre;
        private double _QuantityOfGoodMain;
        private double _QuantityOfGoodPost;
        private double _QuantityOfScrapPost;
        private double _totalCosts = 0;
        private double _NumberOfFailurePre;
        private double _NumberOfFailureMain;
        private double _NumberOfFailurePost;
        private List<Job> _FinishedJobs = new List<Job>();
        private TimeSpan _machineWorkingTimePre = TimeSpan.FromDays(0);
        private TimeSpan _machineWorkingTimeMain = TimeSpan.FromDays(0);
        private TimeSpan _machineWorkingTimePost = TimeSpan.FromDays(0);
        private TimeSpan _downtimePre = TimeSpan.FromDays(0);
        private TimeSpan _downtimeMain = TimeSpan.FromDays(0);
        private TimeSpan _downtimePost = TimeSpan.FromDays(0);
        private TimeSpan _jobExecutionTime = TimeSpan.FromDays(0);
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

        public TimeSpan SetupTimePre { get => _setupTimePre; set => _setupTimePre = value; }
        public double QuantityOfReworkPre { get => _QuantityOfReworkPre; set => _QuantityOfReworkPre = value; }
        public double QuantityOfScrapPre { get => _QuantityOfScrapPre; set => _QuantityOfScrapPre = value; }
        public TimeSpan MachineWorkingTimePre { get => _machineWorkingTimePre; set => _machineWorkingTimePre = value; }
        public TimeSpan MachineWorkingTimeMain { get => _machineWorkingTimeMain; set => _machineWorkingTimeMain = value; }
        public TimeSpan MachineWorkingTimePost { get => _machineWorkingTimePost; set => _machineWorkingTimePost = value; }
        public TimeSpan DowntimePre { get => _downtimePre; set => _downtimePre = value; }
        public TimeSpan DowntimeMain { get => _downtimeMain; set => _downtimeMain = value; }
        public TimeSpan DowntimePost { get => _downtimePost; set => _downtimePost = value; }
        public double QuantityOfReworkMain { get => _QuantityOfReworkMain; set => _QuantityOfReworkMain = value; }
        public double QuantityOfScrapMain { get => _QuantityOfScrapMain; set => _QuantityOfScrapMain = value; }
        public double QuantityOfReworkPost { get => _QuantityOfReworkPost; set => _QuantityOfReworkPost = value; }
        public double QuantityOfScrapPost { get => _QuantityOfScrapPost; set => _QuantityOfScrapPost = value; }
        public TimeSpan SetupTimeMain { get => _setupTimeMain; set => _setupTimeMain = value; }
        public TimeSpan SetupTimePost { get => _setupTimePost; set => _setupTimePost = value; }
        public double QuantityOfGoodPre { get => _QuantityOfGoodPre; set => _QuantityOfGoodPre = value; }
        public double QuantityOfGoodMain { get => _QuantityOfGoodMain; set => _QuantityOfGoodMain = value; }
        public double QuantityOfGoodPost { get => _QuantityOfGoodPost; set => _QuantityOfGoodPost = value; }
        public double NumberOfFailurePre { get => _NumberOfFailurePre; set => _NumberOfFailurePre = value; }
        public double NumberOfFailureMain { get => _NumberOfFailureMain; set => _NumberOfFailureMain = value; }
        public double NumberOfFailurePost { get => _NumberOfFailurePost; set => _NumberOfFailurePost = value; }
        public double Costs { get => _totalCosts; set => _totalCosts = value; }
        public Dictionary<string, double> InputData { get => _InputData; set => _InputData = value; }
        public List<Job> FinishedJobs { get => _FinishedJobs; set => _FinishedJobs = value; }
        public TimeSpan JobExecutionTime { get => _jobExecutionTime; set => _jobExecutionTime = value; }

        public Dictionary<string, double> CalculateKPIs()
        {
            Dictionary<string, double> KPIs = new Dictionary<string, double>();
            double WorkingMinutes = InputData["WorkingHours"] * 60;
            KPIs.Add("AvailabilityPre", (WorkingMinutes - SetupTimePre.TotalMinutes - DowntimePre.TotalMinutes) / WorkingMinutes);
            KPIs.Add("AvailabilityMain", (WorkingMinutes - SetupTimeMain.TotalMinutes - DowntimeMain.TotalMinutes) / WorkingMinutes);
            KPIs.Add("AvailabilityPost", (WorkingMinutes - SetupTimePost.TotalMinutes - DowntimePost.TotalMinutes) / WorkingMinutes);
            KPIs.Add("EffectivenessPre", (MachineWorkingTimePre.TotalMinutes) / (WorkingMinutes - SetupTimePre.TotalMinutes - DowntimePre.TotalMinutes));
            KPIs.Add("EffectivenessMain", (MachineWorkingTimeMain.TotalMinutes) / (WorkingMinutes - SetupTimeMain.TotalMinutes - DowntimeMain.TotalMinutes));
            KPIs.Add("EffectivenessPost", (MachineWorkingTimePost.TotalMinutes) / (WorkingMinutes - SetupTimePost.TotalMinutes - DowntimePost.TotalMinutes));
            KPIs.Add("ThrouputratePre", (QuantityOfGoodPre + QuantityOfReworkPre) / (MachineWorkingTimePre.TotalHours + SetupTimePre.TotalHours + DowntimePre.TotalHours));
            KPIs.Add("ThrouputrateMain", (QuantityOfGoodMain + QuantityOfReworkMain) / (MachineWorkingTimeMain.TotalHours + SetupTimeMain.TotalHours + DowntimeMain.TotalHours));
            KPIs.Add("ThrouputratePost", (QuantityOfGoodPost + QuantityOfReworkPost) / (MachineWorkingTimePost.TotalHours + SetupTimePost.TotalHours + DowntimePost.TotalHours));
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
                KPIs.Add("MTBFPre", (InputData["WorkingHours"] / NumberOfFailurePre)/24 );
                KPIs.Add("MTTRPre", DowntimePre.TotalDays / NumberOfFailurePre);
            }else
            {
                KPIs.Add("MTBFPre", 0);
                KPIs.Add("MTTRPre", 0);
            }
            if (NumberOfFailureMain != 0)
            {
                KPIs.Add("MTBFMain", (InputData["WorkingHours"] / NumberOfFailureMain)/24);
                KPIs.Add("MTTRMain", DowntimeMain.TotalDays / NumberOfFailureMain);
            }
            else
            {
                KPIs.Add("MTBFMain", 0);
                KPIs.Add("MTTRMain", 0);
            }
            if (NumberOfFailurePost!=0)
            {
                KPIs.Add("MTBFPost", (InputData["WorkingHours"] / NumberOfFailurePost)/24);
                KPIs.Add("MTTRPost", DowntimePost.TotalDays / NumberOfFailurePost);
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
            ManufactoringCosts = (3*InputData["WorkingHours"]-(DowntimePre.TotalHours + DowntimeMain.TotalHours + DowntimePost.TotalHours))*InputData["MachineHourCosts"] + (3*InputData["WorkingHours"] - (DowntimePre.TotalHours + DowntimeMain.TotalHours + DowntimePost.TotalHours + MachineWorkingTimePre.TotalHours + MachineWorkingTimeMain.TotalHours + MachineWorkingTimePost.TotalHours)) * InputData["MachineStandHourCosts"];
            foreach (Job j in FinishedJobs)
            {
                foreach (Producttype p in j.Producttype)
                {
                    Revenue += (p.Quantity * p.Price);
                    MaterialCosts += (p.TotalProducedQuantity * p.MaterialCost);
                    foreach (Product pr in p.Products)
                    {
                        LabourCosts += (pr.TotalReworkTime.Minutes /60) * InputData["HourlyWage"];
                    }
                }
            }
            RepairCosts = ((DowntimePre.Add(DowntimeMain).Add(DowntimePost)).TotalHours) * InputData["HourlyWageFitter"];
            Costs = ManufactoringCosts + MaterialCosts + LabourCosts + RepairCosts;

            return Revenue-Costs;
        }
    }
}
