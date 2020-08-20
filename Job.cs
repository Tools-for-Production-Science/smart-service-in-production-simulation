using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Job
    {
        private int _Priority;
        private int _ID;
        private List<Position> _Positions;
        public Job(int priority, int id, List<Position> positions)
        {
            Priority = priority;
            ID = id;
            Positions = positions;
        }

        public int Priority { get => _Priority; set => _Priority = value; }
        public int ID { get => _ID; set => _ID = value; }
        public List<Position> Positions { get => _Positions; set => _Positions = value; }
    }
}
