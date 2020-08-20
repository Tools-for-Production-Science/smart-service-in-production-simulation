using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Postprocess
    {
        Simulation Env;
        TimeSpan RepairTime;

        public Postprocess(Simulation env)
        {
            Env = env;
            // fix
            // hier noch Parameter die eingelesen werden übergeben und setzten

            RepairTime = TimeSpan.FromMinutes(30);
        }

        public IEnumerable<Event> ProductionStep(Simulation env, Resource machine, Request req, int id, TimeSpan productionTime)
        {
            env.Log("{0} ProductNo {1}: Machine Postprocess is in production", env.Now, id);

            yield return env.Timeout(productionTime);
            if (env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenPost = true;
                env.Log("Break Machine Postprocess");
                // Ausfalldauer für M
                yield return env.Timeout(RepairTime);
                env.Log("Machine in Postprocess repaired");
                ProcessControl.BrokenPost = false;
            }
            machine.Release(req);
            env.Log("{0} ProductNo {1}: Machine Postprocess is finished", env.Now, id);
        }

       
       
    }
}
