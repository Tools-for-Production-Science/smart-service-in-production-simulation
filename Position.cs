using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Position
    {
        private int _Quantity;
        private List<Product> _Products;
        private double _ScrapPreMean;
        private double _ScrapMainMean;
        private double _ScrapPostMean;
        private double _ReworkPreMean;
        private double _ReworkMainMean;
        private double _ReworkPostMean;
        private double _MaterialCost;
        private double _Price;
        private TimeSpan _SetupMean;
        private TimeSpan _SetupSigma;
        private int _TotalProducedQuantity = 0;
        private int _ID;

        public Position(int quanity, int id, List<Product> products, double scrapPreMean, double scrapMainMean, double scrapPostMean, double reworkPreMean, double reworkMainMean, double reworkPostMean, TimeSpan setupMean, TimeSpan setupSigma, double materialCost, double price)
        {
            Quantity = quanity;
            ID = id;
            Products = products;
            ScrapPreMean = scrapPreMean;
            ScrapMainMean = scrapMainMean;
            ScrapPostMean = scrapPostMean;
            ReworkPreMean = reworkPreMean;
            ReworkMainMean = reworkMainMean;
            ReworkPostMean = reworkPostMean;
            SetupMean = setupMean;
            SetupSigma = setupSigma;
            MaterialCost = materialCost;
            Price = price;
        }

        public int Quantity { get => _Quantity; set => _Quantity = value; }
        public List<Product> Products { get => _Products; set => _Products = value; }
        public int ID { get => _ID; set => _ID = value; }
        public double ScrapPreMean { get => _ScrapPreMean; set => _ScrapPreMean = value; }
        public double ScrapMainMean { get => _ScrapMainMean; set => _ScrapMainMean = value; }
        public double ScrapPostMean { get => _ScrapPostMean; set => _ScrapPostMean = value; }
        public double ReworkPreMean { get => _ReworkPreMean; set => _ReworkPreMean = value; }
        public double ReworkMainMean { get => _ReworkMainMean; set => _ReworkMainMean = value; }
        public double ReworkPostMean { get => _ReworkPostMean; set => _ReworkPostMean = value; }
        public TimeSpan SetupMean { get => _SetupMean; set => _SetupMean = value; }
        public TimeSpan SetupSigma { get => _SetupSigma; set => _SetupSigma = value; }
        public double MaterialCost { get => _MaterialCost; set => _MaterialCost = value; }
        public double Price { get => _Price; set => _Price = value; }
        public int TotalProducedQuantity { get => _TotalProducedQuantity; set => _TotalProducedQuantity = value; }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   _Quantity == position._Quantity &&
                   EqualityComparer<List<Product>>.Default.Equals(_Products, position._Products) &&
                   _ID == position._ID;
        }

        public override string ToString()
        {
            return "Quantity: " + Quantity;
        }

    }
}
