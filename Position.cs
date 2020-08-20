using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Position
    {
        private int _Quantity;
        private List<Product> _Products;
        private TimeSpan _Setup;
        private TimeSpan _ProductionTimePreProcess;
        private TimeSpan _ProductionTimeMainProcess;
        private TimeSpan _ProductionTimePostProcess;
        private int _ID;

        public Position(int quanity, TimeSpan steup, int id, List<Product> products, TimeSpan productiontimepreprocess, TimeSpan productiontimemainprocess, TimeSpan productiontimepostprocess)
        {
            Quantity = quanity;
            Setup = steup;
            ProductionTimePreProcess = productiontimepreprocess;
            ProductionTimeMainProcess = productiontimemainprocess;
            ProductionTimePostProcess = productiontimepostprocess;
            ID = id;
            Products = products;
        }

        public int Quantity { get => _Quantity; set => _Quantity = value; }
        public List<Product> Products { get => _Products; set => _Products = value; }
        public TimeSpan Setup { get => _Setup; set => _Setup = value; }
        public TimeSpan ProductionTimePreProcess { get => _ProductionTimePreProcess; set => _ProductionTimePreProcess = value; }
        public TimeSpan ProductionTimeMainProcess { get => _ProductionTimeMainProcess; set => _ProductionTimeMainProcess = value; }
        public TimeSpan ProductionTimePostProcess { get => _ProductionTimePostProcess; set => _ProductionTimePostProcess = value; }
        public int ID { get => _ID; set => _ID = value; }
    }
}
