using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Producttype
    {
        private List<Product> _products;
        private TimeSpan _setupMean;
        private TimeSpan _setupSigma;
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

        public Producttype(int quanity, int id, List<Product> products, double scrapPreMean, double scrapMainMean, double scrapPostMean, double reworkPreMean, double reworkMainMean, double reworkPostMean, TimeSpan setupMean, TimeSpan setupSigma, double materialCost, double price)
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

        public int Quantity { get => _quantity; set => _quantity = value; }
        public List<Product> Products { get => _products; set => _products = value; }
        public int ID { get => _id; set => _id = value; }
        public double ScrapPreMean { get => _scrapPreMean; set => _scrapPreMean = value; }
        public double ScrapMainMean { get => _scrapMainMean; set => _scrapMainMean = value; }
        public double ScrapPostMean { get => _scrapPostMean; set => _scrapPostMean = value; }
        public double ReworkPreMean { get => _reworkPreMean; set => _reworkPreMean = value; }
        public double ReworkMainMean { get => _reworkMainMean; set => _reworkMainMean = value; }
        public double ReworkPostMean { get => _reworkPostMean; set => _reworkPostMean = value; }
        public TimeSpan SetupMean { get => _setupMean; set => _setupMean = value; }
        public TimeSpan SetupSigma { get => _setupSigma; set => _setupSigma = value; }
        public double MaterialCost { get => _materialCost; set => _materialCost = value; }
        public double Price { get => _price; set => _price = value; }
        public int TotalProducedQuantity { get => _totalProducedQuantity; set => _totalProducedQuantity = value; }

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
