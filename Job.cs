using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Job
    {
        private int _priority;
        private int _id;
        private List<Producttype> _positions;

        public Job(int priority, int id, List<Producttype> positions)
        {
            Priority = priority;
            ID = id;
            Positions = positions;
        }


        public int Priority { get => _priority; set => _priority = value; }
        public int ID { get => _id; set => _id = value; }
        public List<Producttype> Positions { get => _positions; set => _positions = value; }

    }
}
