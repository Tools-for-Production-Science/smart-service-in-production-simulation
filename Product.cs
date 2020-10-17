using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Product
    {
        private int _ID;
        private Boolean _broken = false;
        private TimeSpan _ReworkTimeMean;
        private TimeSpan _ReworkTimeSigma;
        private TimeSpan _ProductionTimePreMean;
        private TimeSpan _ProductionTimeMainMean;
        private TimeSpan _ProductionTimePostMean;
        private TimeSpan _ProductionTimePreSigma;
        private TimeSpan _ProductionTimeMainSigma;
        private TimeSpan _ProductionTimePostSigma;
        private TimeSpan _TotalReworkTime;

        public Product(int id, TimeSpan productiontimepremean, TimeSpan productiontimemainmean, TimeSpan productiontimepostmean, TimeSpan productionTimePreSigma, TimeSpan productionTimeMainSigma, TimeSpan productionTimePostSigma, TimeSpan reworkTimeMean, TimeSpan reworkTimeSigma)
        {
            ID = id;
            ReworkTimeMean = reworkTimeMean;            
            ReworkTimeSigma = reworkTimeSigma;
            ProductionTimePreMean = productiontimepremean;
            ProductionTimeMainMean = productiontimemainmean;
            ProductionTimePostMean = productiontimepostmean;
            ProductionTimePreSigma = productionTimePreSigma;
            ProductionTimeMainSigma = productionTimeMainSigma;
            ProductionTimePostSigma = productionTimePostSigma;
            TotalReworkTime = TimeSpan.FromDays(0);
        }

        public int ID { get => _ID; set => _ID = value; }
        public bool Broken { get => _broken; set => _broken = value; }
        public TimeSpan ReworkTimeMean { get => _ReworkTimeMean; set => _ReworkTimeMean = value; }        
        public TimeSpan ReworkTimeSigma { get => _ReworkTimeSigma; set => _ReworkTimeSigma = value; }
        public TimeSpan ProductionTimePreMean { get => _ProductionTimePreMean; set => _ProductionTimePreMean = value; }
        public TimeSpan ProductionTimeMainMean { get => _ProductionTimeMainMean; set => _ProductionTimeMainMean = value; }
        public TimeSpan ProductionTimePostMean { get => _ProductionTimePostMean; set => _ProductionTimePostMean = value; }
        public TimeSpan ProductionTimePreSigma { get => _ProductionTimePreSigma; set => _ProductionTimePreSigma = value; }
        public TimeSpan ProductionTimeMainSigma { get => _ProductionTimeMainSigma; set => _ProductionTimeMainSigma = value; }  
        public TimeSpan ProductionTimePostSigma { get => _ProductionTimePostSigma; set => _ProductionTimePostSigma = value; }
        public TimeSpan TotalReworkTime { get => _TotalReworkTime; set => _TotalReworkTime = value; }
    }
}
