namespace ProduktionssystemSimulation
{
    public class SmartService
    {
        private double _ssEffectDowntimeMean = 0;
        private double _ssEffectMTBFMean = 0;
        private double _ssEffectScrap = 0;
        private double _ssEffectRework = 0;
        private double _ssEffectDowntimeSigma = 0;

        public SmartService(double scrap, double mTBFMean, double downtimeMean, double downtimeSigma, double rework)
        {
            SSEffectScrap = scrap;
            SSEffectMTBFMean = mTBFMean;
            SSEffectDowntimeMean = downtimeMean;
            SSEffectRework = rework;
            SSEffectDowntimeSigma = downtimeSigma;
        }
        
        public double SSEffectDowntimeMean { get => _ssEffectDowntimeMean; set => _ssEffectDowntimeMean = value; }
        public double SSEffectScrap { get => _ssEffectScrap; set => _ssEffectScrap = value; }
        public double SSEffectMTBFMean { get => _ssEffectMTBFMean; set => _ssEffectMTBFMean = value; }
        public double SSEffectRework { get => _ssEffectRework; set => _ssEffectRework = value; }
        public double SSEffectDowntimeSigma { get => _ssEffectDowntimeSigma; set => _ssEffectDowntimeSigma = value; }
    }
}
