using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Product
    {
        private int _ID;
        private Boolean _broken = false;
        private double _ScrapPreMean;
        private double _ScrapPreSigma;
        private double _ScrapMainMean;
        private double _ScrapMainSigma;
        private double _ScrapPostMean;
        private double _ScrapPostSigma;
        private TimeSpan _ProductionTimePreMean;
        private TimeSpan _ProductionTimeMainMean;
        private TimeSpan _ProductionTimePostMean;
        private TimeSpan _ProductionTimePreSigma;
        private TimeSpan _ProductionTimeMainSigma;
        private TimeSpan _ProductionTimePostSigma;
        public Product(int id, TimeSpan productiontimepremean, TimeSpan productiontimemainmean, TimeSpan productiontimepostmean, TimeSpan productionTimePreSigma, TimeSpan productionTimeMainSigma, TimeSpan productionTimePostSigma, double spoiltPreMean, double spoiltPreSigma, double spoiltMainMean, double spoiltMainSigma, double spoiltPostMean, double spoiltPostSigma)
        {
            ID = id;
            ProductionTimePreMean = productiontimepremean;
            ProductionTimeMainMean = productiontimemainmean;
            ProductionTimePostMean = productiontimepostmean;
            ProductionTimePreSigma = productionTimePreSigma;
            ProductionTimeMainSigma = productionTimeMainSigma;
            ProductionTimePostSigma = productionTimePostSigma;
            ScrapPreMean = spoiltPreMean;
            ScrapPreSigma = spoiltPreSigma;
            ScrapMainMean = spoiltMainMean;
            ScrapMainSigma = spoiltMainSigma;
            ScrapPostMean = spoiltPostMean;
            ScrapPostSigma = spoiltPostSigma;
            
        }

        public int ID { get => _ID; set => _ID = value; }
        public bool Broken { get => _broken; set => _broken = value; }
        public TimeSpan ProductionTimePreMean { get => _ProductionTimePreMean; set => _ProductionTimePreMean = value; }
        public TimeSpan ProductionTimeMainMean { get => _ProductionTimeMainMean; set => _ProductionTimeMainMean = value; }
        public TimeSpan ProductionTimePostMean { get => _ProductionTimePostMean; set => _ProductionTimePostMean = value; }
        public TimeSpan ProductionTimePreSigma { get => _ProductionTimePreSigma; set => _ProductionTimePreSigma = value; }
        public TimeSpan ProductionTimeMainSigma { get => _ProductionTimeMainSigma; set => _ProductionTimeMainSigma = value; }  
        public TimeSpan ProductionTimePostSigma { get => _ProductionTimePostSigma; set => _ProductionTimePostSigma = value; }
        public double ScrapPreMean { get => _ScrapPreMean; set => _ScrapPreMean = value; }
        public double ScrapPreSigma { get => _ScrapPreSigma; set => _ScrapPreSigma = value; }
        public double ScrapMainMean { get => _ScrapMainMean; set => _ScrapMainMean = value; }
        public double ScrapMainSigma { get => _ScrapMainSigma; set => _ScrapMainSigma = value; }
        public double ScrapPostMean { get => _ScrapPostMean; set => _ScrapPostMean = value; }
        public double ScrapPostSigma { get => _ScrapPostSigma; set => _ScrapPostSigma = value; }
    }
}
