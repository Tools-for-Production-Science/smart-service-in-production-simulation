using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Producttype
    {
        private List<Product> _products;
        private TimeSpan _setupPreMean;
        private TimeSpan _setupPreSigma;
        private TimeSpan _setupMainMean;
        private TimeSpan _setupMainSigma;
        private TimeSpan _setupPostMean;
        private TimeSpan _setupPostSigma;
        private double _scrapPreMean;
        private double _scrapMainMean;
        private double _scrapPostMean;
        private double _reworkPreMean;
        private double _reworkMainMean;
        private double _reworkPostMean;
        private double _materialCost;
        private double _price;
        private int _quantity;
        private int _totalProducedQuantity;
        private int _id;

        public Producttype(int quanity, int id, List<Product> products, double scrapPreMean, double scrapMainMean, double scrapPostMean, double reworkPreMean, double reworkMainMean, double reworkPostMean, TimeSpan setupMean, TimeSpan setupSigma, double materialCost, double price, TimeSpan setupMainMean, TimeSpan setupMainSigma, TimeSpan setupPostMean, TimeSpan setupPostSigma)
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
            SetupPreMean = setupMean;
            SetupPreSigma = setupSigma;
            MaterialCost = materialCost;
            Price = price;
            SetupMainMean = setupMainMean;
            SetupMainSigma = setupMainSigma;
            SetupPostMean = setupPostMean;
            SetupPostSigma = setupPostSigma;
        }

        public int Quantity { get => _quantity; set => _quantity = value; }
        public List<Product> Products { get => _products; set => _products = value; }
        public int ID { get => _id; set => _id = value; }
        public double ScrapPreMean { get => _scrapPreMean; set => _scrapPreMean = value; }
        public double ScrapMainMean { get => _scrapMainMean; set => _scrapMainMean = value; }
        public double ScrapPostMean { get => _scrapPostMean; set => _scrapPostMean = value; }
        public double ReworkPreMean { get => _reworkPreMean; set => _reworkPreMean = value; }
        public double ReworkMainMean { get => _reworkMainMean; set => _reworkMainMean = value; }
        public double ReworkPostMean { get => _reworkPostMean; set => _reworkPostMean = value; }
        public TimeSpan SetupPreMean { get => _setupPreMean; set => _setupPreMean = value; }
        public TimeSpan SetupPreSigma { get => _setupPreSigma; set => _setupPreSigma = value; }
        public double MaterialCost { get => _materialCost; set => _materialCost = value; }
        public double Price { get => _price; set => _price = value; }
        public int TotalProducedQuantity { get => _totalProducedQuantity; set => _totalProducedQuantity = value; }
        public TimeSpan SetupMainMean { get => _setupMainMean; set => _setupMainMean = value; }
        public TimeSpan SetupMainSigma { get => _setupMainSigma; set => _setupMainSigma = value; }
        public TimeSpan SetupPostMean { get => _setupPostMean; set => _setupPostMean = value; }
        public TimeSpan SetupPostSigma { get => _setupPostSigma; set => _setupPostSigma = value; }

        public override bool Equals(object obj)
        {
            return obj is Producttype producttype &&
                   _quantity == producttype._quantity &&
                   EqualityComparer<List<Product>>.Default.Equals(_products, producttype._products) &&
                   _id == producttype._id;
        }

        public override string ToString()
        {
            return "Quantity: " + Quantity;
        }

    }
}
