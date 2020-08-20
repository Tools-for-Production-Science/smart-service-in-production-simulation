using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Product
    {
        private int _ID;
        private Boolean _broken = false;
        public Product( int id)
        {
            ID = id;
        }

        public int ID { get => _ID; set => _ID = value; }
        public bool Broken { get => _broken; set => _broken = value; }
    }
}
