using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    public class Preprocess
    {
        Simulation env;
        TimeSpan RepairTime;
       
        public Preprocess(Simulation env)
        {
            this.env = env;
            // fix
            // hier noch Parameter die eingelesen werden übergeben und setzten
            RepairTime = TimeSpan.FromMinutes(30);
        }

        public IEnumerable<Event> ProductionStep(Simulation env, Resource machine, Request req, int id, TimeSpan productionTime)
        {
            env.Log("{0} ProductNo {1}: Machine Preprocess is in production", env.Now, id);

            yield return env.Timeout(productionTime);
            if (env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenPre = true;
                env.Log("Break Machine in Preprocess");
                // Ausfalldauer für M
                yield return env.Timeout(RepairTime);
                env.Log("Machine in Preprocess repaired");
                ProcessControl.BrokenPre = false;
            }
            machine.Release(req);
            env.Log("{0} ProductNo {1}: Machine Preprocess is finished", env.Now, id);
        }

        
    }
}
