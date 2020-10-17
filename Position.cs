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
        private double _TotalProductionTime;
        private int _TotalProducedQuantity = 0;
        private int _NumberScrapPre = 0;
        private int _NumberReworkPre = 0;
        private int _NumberScrapMain = 0;
        private int _NumberReworkMain = 0;
        private int _NumberScrapPost = 0;
        private int _NumberReworkPost = 0;
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
        public int NumberReworkPre { get => _NumberReworkPre; set => _NumberReworkPre = value; }
        public int NumberScrapPre { get => _NumberScrapPre; set => _NumberScrapPre = value; }
        public TimeSpan SetupMean { get => _SetupMean; set => _SetupMean = value; }
        public TimeSpan SetupSigma { get => _SetupSigma; set => _SetupSigma = value; }
        public double MaterialCost { get => _MaterialCost; set => _MaterialCost = value; }
        public int NumberScrapMain { get => _NumberScrapMain; set => _NumberScrapMain = value; }
        public int NumberReworkMain { get => _NumberReworkMain; set => _NumberReworkMain = value; }
        public int NumberScrapPost { get => _NumberScrapPost; set => _NumberScrapPost = value; }
        public int NumberReworkPost { get => _NumberReworkPost; set => _NumberReworkPost = value; }
        public double Price { get => _Price; set => _Price = value; }
        public double TotalProductionTime { get => _TotalProductionTime; set => _TotalProductionTime = value; }
        public int TotalProducedQuantity { get => _TotalProducedQuantity; set => _TotalProducedQuantity = value; }
    }
}
