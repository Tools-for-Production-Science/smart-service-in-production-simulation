using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    class SmartService
    {
        private static double _effektausmaß;
        private static double _anschaffungskosten;
        private static double _fixkosten;
        private static double _varKosten;
        private static double _preis;
        private static double _mindAbnahme;
        private static double _maxAbnahme;
        private static int _ausfallwkeit = 0;
        private static int _maschinenReparaturzeit = 0;
        private static int _ausschuss = 0;
        private static int _leerlauf = 0;
        private static TimeSpan _laufzeit;

        public double Effektausmaß { get => _effektausmaß; set => _effektausmaß = value; }
        public double Anschaffungskosten { get => _anschaffungskosten; set => _anschaffungskosten = value; }
        public double Fixkosten { get => _fixkosten; set => _fixkosten = value; }
        public double VariableKosten { get => _varKosten; set => _varKosten = value; }
        public double Preis { get => _preis; set => _preis = value; }
        public double MindAbnahme { get => _mindAbnahme; set => _mindAbnahme = value; }
        public double MaxAbnahme { get => _maxAbnahme; set => _maxAbnahme = value; }

        public int MaschinenAusfallwkeit { get => _ausfallwkeit; set => _ausfallwkeit = value; }
        public int Ausschuss { get => _ausschuss; set => _ausschuss = value; }
        public int MaschinenReparaturzeit { get => _maschinenReparaturzeit; set => _maschinenReparaturzeit = value; }
        public int Leerlaufzeit { get => _leerlauf; set => _leerlauf = value; }

        public TimeSpan Laufzeit { get => _laufzeit; set => _laufzeit = value; }

        public SmartService(double effektausmaß, double fixkosten, double varKosten, TimeSpan laufzeit, double preis, double mindAbnahme, double maxAbnahme)
        {
            Effektausmaß = effektausmaß;
            Fixkosten = fixkosten;
            VariableKosten = varKosten;
            Laufzeit = laufzeit;
            Preis = preis;
            MindAbnahme = mindAbnahme;
            MaxAbnahme = maxAbnahme;
        }
    }
}
