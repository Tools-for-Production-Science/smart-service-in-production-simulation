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
        public override string ToString()
        {
            String s = "ID: " + ID + "\nPriority: " + Priority + "\n";
            foreach(Position p in Positions)
            {
                s = s + p.ToString() + "\n";
            }

            return s;
        }

        public override bool Equals(object obj)
        {
            return obj is Job job &&
                   _Priority == job._Priority &&
                   _ID == job._ID &&
                   EqualityComparer<List<Position>>.Default.Equals(_Positions, job._Positions);
        }

        public int Priority { get => _Priority; set => _Priority = value; }
        public int ID { get => _ID; set => _ID = value; }
        public List<Position> Positions { get => _Positions; set => _Positions = value; }

    }
}
