using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Position
    {
        private int _QuantityMean;
        private int _QuantitySigma;
        private List<Product> _Products;
        private double _ScrapPreMean;
        private double _ScrapMainMean;
        private double _ScrapPostMean;
        private double _ReworkPreMean;
        private double _ReworkMainMean;
        private double _ReworkPostMean;
        private TimeSpan _SetupPreMean;
        private TimeSpan _SetupPreSigma;
        private TimeSpan _SetupMainMean;
        private TimeSpan _SetupMainSigma;
        private TimeSpan _SetupPostMean;
        private TimeSpan _SetupPostSigma;

        private int _ID;

        public Position(int quanityMean, int quantitySigma, int id, List<Product> products, double scrapPreMean, double scrapMainMean, double scrapPostMean, double reworkPreMean, double reworkMainMean, double reworkPostMean, TimeSpan setupMean, TimeSpan setupSigma, TimeSpan setupMainMean, TimeSpan setupMainSigma, TimeSpan setupPostMean, TimeSpan setupPostSigma)
        {
            QuantityMean = quanityMean;
            QuantitySigma = quantitySigma;
            ID = id;
            Products = products;
            ScrapPreMean = scrapPreMean;
            ScrapMainMean = scrapMainMean;
            ScrapPostMean = scrapPostMean;
            ReworkPreMean = reworkPreMean;
            ReworkMainMean = reworkMainMean;
            ReworkPostMean = reworkPostMean;
            SetupPreMean = setupMean;
            SetupPreSigma = setupSigma;
            SetupMainMean = setupMainMean;
            SetupMainSigma = setupMainSigma;
            SetupPostMean = setupPostMean;
            SetupPostSigma = setupPostSigma;

        }

        public int QuantityMean { get => _QuantityMean; set => _QuantityMean = value; }
        public int QuantitySigma { get => _QuantitySigma; set => _QuantitySigma = value; }
        public List<Product> Products { get => _Products; set => _Products = value; }
        public int ID { get => _ID; set => _ID = value; }
        public double ScrapPreMean { get => _ScrapPreMean; set => _ScrapPreMean = value; }
        public double ScrapMainMean { get => _ScrapMainMean; set => _ScrapMainMean = value; }
        public double ScrapPostMean { get => _ScrapPostMean; set => _ScrapPostMean = value; }
        public double ReworkPreMean { get => _ReworkPreMean; set => _ReworkPreMean = value; }
        public double ReworkMainMean { get => _ReworkMainMean; set => _ReworkMainMean = value; }
        public double ReworkPostMean { get => _ReworkPostMean; set => _ReworkPostMean = value; }
        public TimeSpan SetupPreMean { get => _SetupPreMean; set => _SetupPreMean = value; }
        public TimeSpan SetupPreSigma { get => _SetupPreSigma; set => _SetupPreSigma = value; }
        public TimeSpan SetupMainMean { get => _SetupMainMean; set => _SetupMainMean = value; }
        public TimeSpan SetupMainSigma { get => _SetupMainSigma; set => _SetupMainSigma = value; }
        public TimeSpan SetupPostMean { get => _SetupPostMean; set => _SetupPostMean = value; }
        public TimeSpan SetupPostSigma { get => _SetupPostSigma; set => _SetupPostSigma = value; }
    }
}
