namespace ProduktionssystemSimulation
{
    public class SmartService
    {
        private double _DowntimeMean = 0;
        private double _MTBFMean = 0;
        private double _Scrap = 0;
        private double _Rework= 0;
        private double _DowntimeSigma = 0;

        public SmartService(double scrap, double mTBFMean, double downtimeMean, double downtimeSigma, double rework)
        {
            Scrap = scrap;
            MTBFMean = mTBFMean;
            DowntimeMean = downtimeMean;
            Rework = rework;
            DowntimeSigma = downtimeSigma;
        }
        
        public double DowntimeMean { get => _DowntimeMean; set => _DowntimeMean = value; }
        public double Scrap { get => _Scrap; set => _Scrap = value; }
        public double MTBFMean { get => _MTBFMean; set => _MTBFMean = value; }
        public double Rework { get => _Rework; set => _Rework = value; }
        public double DowntimeSigma { get => _DowntimeSigma; set => _DowntimeSigma = value; }
    }
}
