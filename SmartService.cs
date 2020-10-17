using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class SmartService
    {
        private static double _probabilityOfOccurrence;
        private static double _investmentCosts;
        private static double _fixcosts;
        private static double _varCosts;
        private static double _price;
        private static double _mindCapture;
        private static double _maxCapture;
        private static double _downtime = 0;
        private static double _mttf = 0;
        private static double _scrap = 0;
        private static double _rework= 0;
        private static TimeSpan _duration;

        public double ProbabilityOfOccurrence { get => _probabilityOfOccurrence; set => _probabilityOfOccurrence = value; }
        public double InvestmentCosts { get => _investmentCosts; set => _investmentCosts = value; }
        public double Fixcosts { get => _fixcosts; set => _fixcosts = value; }
        public double VariableCosts { get => _varCosts; set => _varCosts = value; }
        public double Price { get => _price; set => _price = value; }
        public double MindCapture { get => _mindCapture; set => _mindCapture = value; }
        public double MaxCapture { get => _maxCapture; set => _maxCapture = value; }
        public double Downtime { get => _downtime; set => _downtime = value; }
        public double Scrap { get => _scrap; set => _scrap = value; }
        public double MTTF { get => _mttf; set => _mttf = value; }
        public TimeSpan Duration { get => _duration; set => _duration = value; }
        public double Rework { get => _rework; set => _rework = value; }

        public SmartService(double probabilityOfOccurrence, double fixcosts, double varCosts, TimeSpan duration, double price, double mindCapture, double maxCapture, double investmentCosts, double scrap, double mTTF, double downtime, double rework)
        {
            ProbabilityOfOccurrence = probabilityOfOccurrence;
            Fixcosts = fixcosts;
            VariableCosts = varCosts;
            Duration = duration;
            Price = price;
            MindCapture = mindCapture;
            MaxCapture = maxCapture;
            Scrap = scrap;
            MTTF = mTTF;
            Downtime = downtime;
            Rework = rework;
            InvestmentCosts = investmentCosts;
        }
    }
}
