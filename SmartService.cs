namespace ProduktionssystemSimulation
{
    public class SmartService
    {
        private double _downtimeMean = 0;
        private double _mTBFMean = 0;
        private double _scrap = 0;
        private double _rework = 0;
        private double _downtimeSigma = 0;

        public SmartService(double scrap, double mTBFMean, double downtimeMean, double downtimeSigma, double rework)
        {
            Scrap = scrap;
            MTBFMean = mTBFMean;
            DowntimeMean = downtimeMean;
            Rework = rework;
            DowntimeSigma = downtimeSigma;
        }
        
        public double DowntimeMean { get => _downtimeMean; set => _downtimeMean = value; }
        public double Scrap { get => _scrap; set => _scrap = value; }
        public double MTBFMean { get => _mTBFMean; set => _mTBFMean = value; }
        public double Rework { get => _rework; set => _rework = value; }
        public double DowntimeSigma { get => _downtimeSigma; set => _downtimeSigma = value; }
    }
}
