using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class SmartService
    {
        private static double _downtime = 0;
        private static double _mttf = 0;
        private static double _scrap = 0;
        private static double _rework= 0;


        public double Downtime { get => _downtime; set => _downtime = value; }
        public double Scrap { get => _scrap; set => _scrap = value; }
        public double MTTF { get => _mttf; set => _mttf = value; }
        public double Rework { get => _rework; set => _rework = value; }

        public SmartService( double scrap, double mTTF, double downtime, double rework)
        {
            Scrap = scrap;
            MTTF = mTTF;
            Downtime = downtime;
            Rework = rework;
        }
    }
}
