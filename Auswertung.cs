using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    class Auswertung
    {
        private TimeSpan _ProductionTime;
        private TimeSpan _Downtime;
        private TimeSpan _PlanedRuntimePerProduct;
        private TimeSpan _PlanedOperatingHours;
        private TimeSpan _TotalOperatingTime;
        private TimeSpan _Setup;
        private TimeSpan _JobExecutionTime;
        private int _QuantityOfDefects;
        private int _QuantityOfScrap;
        private int _AmountOfFailures;
        private int _TotalQuantity;
        private double Availability;
        private double Effectiveness;
        private double MachineCapacity;
        private double MTBF;
        private double MTTR;
        private double OEE;
        private double QualityBuyRate;
        private double ScrapRatio;
        private double ReworkRatio;
        private double ReworkToDefectsRatio;
        private double ThroughputRatio;

        public Auswertung(TimeSpan productiontime, TimeSpan downtime, TimeSpan planedruntimeperproduct, TimeSpan planedoperatinghours, TimeSpan totaloperatingtime, TimeSpan setup, TimeSpan jobexecutiontime, int defects, int scrap, int failures, int quantity)
        {
            ProductionTime = productiontime;
            Downtime = downtime;
            PlanedRuntimePerProduct = planedruntimeperproduct;
            PlanedOperatingHours = planedoperatinghours;
            TotalOperatingTime = totaloperatingtime;
            Setup = setup;
            JobExecutionTime = jobexecutiontime;
            QuantityOfDefects = defects;
            QuantityOfScrap = scrap;
            AmountOfFailures = failures;
            TotalQuantity = quantity;
        }

        public TimeSpan ProductionTime { get => _ProductionTime; set => _ProductionTime = value; }
        public TimeSpan Downtime { get => _Downtime; set => _Downtime = value; }
        public TimeSpan PlanedRuntimePerProduct { get => _PlanedRuntimePerProduct; set => _PlanedRuntimePerProduct = value; }
        public int TotalQuantity { get => _TotalQuantity; set => _TotalQuantity = value; }
        public TimeSpan PlanedOperatingHours { get => _PlanedOperatingHours; set => _PlanedOperatingHours = value; }
        public TimeSpan TotalOperatingTime { get => _TotalOperatingTime; set => _TotalOperatingTime = value; }
        public int AmountOfFailures { get => _AmountOfFailures; set => _AmountOfFailures = value; }
        public TimeSpan Setup { get => _Setup; set => _Setup = value; }
        public int QuantityOfDefects { get => _QuantityOfDefects; set => _QuantityOfDefects = value; }
        public int QuantityOfScrap { get => _QuantityOfScrap; set => _QuantityOfScrap = value; }
        public TimeSpan JobExecutionTime { get => _JobExecutionTime; set => _JobExecutionTime = value; }

        public void CalculateKPIs(Auswertung auswertung)
        {
            auswertung.Availability = (auswertung.ProductionTime - auswertung.Setup - auswertung.Downtime) / (auswertung.ProductionTime + auswertung.Setup + auswertung.Downtime);
            auswertung.Effectiveness = (auswertung.PlanedRuntimePerProduct * auswertung.TotalQuantity) / auswertung.ProductionTime;
            auswertung.MachineCapacity = (auswertung.TotalQuantity*auswertung.PlanedRuntimePerProduct) / auswertung.PlanedOperatingHours;
            //auswertung.MTBF = auswertung.TotalOperatingTime / auswertung.AmountOfFailures;
            //auswertung.MTTR = auswertung.Downtime / auswertung.AmountOfFailures;
            auswertung.QualityBuyRate = (auswertung.TotalQuantity - auswertung.QuantityOfDefects) / auswertung.TotalQuantity;
            auswertung.ScrapRatio = auswertung.QuantityOfScrap / auswertung.TotalQuantity;
            auswertung.ReworkRatio = (auswertung.QuantityOfDefects - auswertung.QuantityOfScrap) / auswertung.TotalQuantity;
            auswertung.OEE = auswertung.Availability * auswertung.Effectiveness * auswertung.QualityBuyRate;
        }
    }
}
