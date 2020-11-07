namespace ProduktionssystemSimulation
{
    public class SmartService
    {
        private double _smartServiceEffectDowntimeMean = 0;
        private double _smartServiceEffectMTBFMean = 0;
        private double _smartServiceEffectScrap = 0;
        private double _smartServiceEffectRework = 0;
        private double _smartServiceEffectDowntimeSigma = 0;

        public SmartService(double scrap, double mTBFMean, double downtimeMean, double downtimeSigma, double rework)
        {
            smartServiceEffectScrap = scrap;
            smartServiceEffectMTBFMean = mTBFMean;
            smartServiceEffectDowntimeMean = downtimeMean;
            smartServiceEffectRework = rework;
            smartServiceEffectDowntimeSigma = downtimeSigma;
        }
        
        public double smartServiceEffectDowntimeMean { get => _smartServiceEffectDowntimeMean; set => _smartServiceEffectDowntimeMean = value; }
        public double smartServiceEffectScrap { get => _smartServiceEffectScrap; set => _smartServiceEffectScrap = value; }
        public double smartServiceEffectMTBFMean { get => _smartServiceEffectMTBFMean; set => _smartServiceEffectMTBFMean = value; }
        public double smartServiceEffectRework { get => _smartServiceEffectRework; set => _smartServiceEffectRework = value; }
        public double smartServiceEffectDowntimeSigma { get => _smartServiceEffectDowntimeSigma; set => _smartServiceEffectDowntimeSigma = value; }
    }
}
