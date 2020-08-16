using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Position
    {
        public int Quantity;
        public List<Product> Products;
        public TimeSpan Setup;
        public TimeSpan ProductionTimePreProcess;
        public TimeSpan sroductionTimeMainProcess;
        public TimeSpan ProductionTimePostProcess;
        public int ID;

        public Position(int quanity, TimeSpan steup, int id, List<Product> products, TimeSpan productiontimepreprocess, TimeSpan productiontimemainprocess, TimeSpan productiontimepostprocess)
        {
            Quantity = quanity;
            Setup = steup;
            ProductionTimePreProcess = productiontimepreprocess;
            sroductionTimeMainProcess = productiontimemainprocess;
            ProductionTimePostProcess = productiontimepostprocess;
            ID = id;
            Products = products;
        }
    }
}
