using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    public class Preprocess
    {
        Simulation Env;
        double DowntimePreMean;
        double DowntimePreSigma;
       
        public Preprocess(Simulation env, double downtimepremean, double downtimepresigma)
        {
            Env = env;
            // fix
            // hier noch Parameter die eingelesen werden übergeben und setzten
            DowntimePreMean = downtimepremean;
            DowntimePreSigma = downtimepresigma;
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
                yield return env.Timeout(TimeSpan.FromDays(env.RandNormalPositive(DowntimePreMean, DowntimePreSigma)));
                env.Log("Machine in Preprocess repaired");
                ProcessControl.BrokenPre = false;
            }
            machine.Release(req);
            env.Log("{0} ProductNo {1}: Machine Preprocess is finished", env.Now, product.ID);
        }
    }
}
