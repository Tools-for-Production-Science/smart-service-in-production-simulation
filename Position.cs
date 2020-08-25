using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Position
    {
        private int _Quantity;
        private List<Product> _Products;
        private TimeSpan _SetupPreMean;
        private TimeSpan _SetupPreSigma;
        private TimeSpan _SetupMainMean;
        private TimeSpan _SetupMainSigma;
        private TimeSpan _SetupPostMean;
        private TimeSpan _SetupPostSigma;

        private int _ID;

        public Position(int quanity, int id, List<Product> products, TimeSpan setupMean, TimeSpan setupSigma, TimeSpan setupMainMean, TimeSpan setupMainSigma, TimeSpan setupPostMean, TimeSpan setupPostSigma)
        {
            Quantity = quanity;
            ID = id;
            Products = products; 
            SetupPreMean = setupMean;
            SetupPreSigma = setupSigma;
            SetupMainMean = setupMainMean;
            SetupMainSigma = setupMainSigma;
            SetupPostMean = setupPostMean;
            SetupPostSigma = setupPostSigma;
        }

        public int Quantity { get => _Quantity; set => _Quantity = value; }
        public List<Product> Products { get => _Products; set => _Products = value; }
        public int ID { get => _ID; set => _ID = value; }
        public TimeSpan SetupPreMean { get => _SetupPreMean; set => _SetupPreMean = value; }
        public TimeSpan SetupPreSigma { get => _SetupPreSigma; set => _SetupPreSigma = value; }
        public TimeSpan SetupMainMean { get => _SetupMainMean; set => _SetupMainMean = value; }
        public TimeSpan SetupMainSigma { get => _SetupMainSigma; set => _SetupMainSigma = value; }
        public TimeSpan SetupPostMean { get => _SetupPostMean; set => _SetupPostMean = value; }
        public TimeSpan SetupPostSigma { get => _SetupPostSigma; set => _SetupPostSigma = value; }
    }
}
