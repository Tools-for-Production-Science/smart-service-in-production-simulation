using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    public class Preprocess
    {
        Simulation Env;
        TimeSpan RepairTime;
       
        public Preprocess(Simulation env)
        {
            Env = env;
            // fix
            // hier noch Parameter die eingelesen werden übergeben und setzten
            RepairTime = TimeSpan.FromMinutes(30);
        }

        public IEnumerable<Event> ProductionStep(Simulation env, Resource machine, Request req, Product product)
        {
            env.Log("{0} ProductNo {1}: Machine Preprocess is in production", env.Now, product.ID);
            yield return env.TimeoutNormalPositive(product.ProductionTimePreMean, product.ProductionTimePreSigma);
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
            env.Log("{0} ProductNo {1}: Machine Preprocess is finished", env.Now, product.ID);
        }
    }
}
