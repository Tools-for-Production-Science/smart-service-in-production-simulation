using System;
using System.Collections.Generic;
using System.Text;

namespace ProductionsystemSimulation
{
    public class Job
    {
        private int _priority;
        private int _id;
        private List<Producttype> _producttype;

        public Job(int priority, int id, List<Producttype> producttype)
        {
            Priority = priority;
            ID = id;
            Producttype = producttype;
        }


        public int Priority { get => _priority; set => _priority = value; }
        public int ID { get => _id; set => _id = value; }
        public List<Producttype> Producttype { get => _producttype; set => _producttype = value; }

    }
}
