using System;
using System.Collections.Generic;
using System.Text;

namespace ProduktionssystemSimulation
{
    public class Job
    {
        public int Priority;
        public int ID;
        public List<Position> Positions;
        public Job(int priority, int id, List<Position> positions)
        {
            Priority = priority;
            ID = id;
            Positions = positions;
        }
    }
}
