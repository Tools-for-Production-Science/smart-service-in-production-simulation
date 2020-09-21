using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Postprocess
    {
        Simulation Env;
        TimeSpan RepairTime;
        double DowntimePreMean;
        double DowntimePreSigma;

        public Postprocess(Simulation env, double downtimepremean, double downtimepresigma)
        {
            Env = env;
            DowntimePreMean = downtimepremean;
            DowntimePreSigma = downtimepresigma;
        }

        public IEnumerable<Event> ProductionStep(Simulation env, Resource machine, Request req, Product product)
        {
            env.Log("{0} ProductNo {1}: Machine Postprocess is in production", env.Now, product.ID);

            yield return env.TimeoutNormalPositive(product.ProductionTimePostMean, product.ProductionTimePostSigma);
            if (env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenPost = true;
                env.Log("Break Machine Postprocess");
                // Ausfalldauer für M
                yield return env.Timeout(TimeSpan.FromDays(env.RandNormalPositive(DowntimePreMean, DowntimePreSigma)));
                env.Log("Machine in Postprocess repaired");
                ProcessControl.BrokenPost = false;
            }
            machine.Release(req);
            env.Log("{0} ProductNo {1}: Machine Postprocess is finished", env.Now, product.ID);
        }

       
       
    }
}
