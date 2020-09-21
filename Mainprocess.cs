using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Mainprocess
    {
        Simulation Env;
        TimeSpan Downtime;
        SmartService SmartService;
        double DowntimeMainMean;
        double DowntimeMainSigma;

        public Mainprocess(Simulation env, SmartService smartService, double downtimemainmean, double downtimemainsigma)
        {
            Env = env;
            SmartService = smartService;
            DowntimeMainMean = downtimemainmean;
            DowntimeMainSigma = downtimemainsigma;
        }

        public IEnumerable<Event> ProductionStep(Simulation env, Resource machine, Request req, Product product)
        {
            env.Log("{0} ProductNo {1}: Machine Mainprocess is in production", env.Now, product.ID);

            yield return env.TimeoutNormalPositive(product.ProductionTimeMainMean, product.ProductionTimeMainSigma);
            if (env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenMain = true;
                env.Log("Break Machine in Mainprocess");
                // Ausfalldauer für M
                Downtime = TimeSpan.FromMinutes(env.RandNormalPositive(DowntimeMainMean,DowntimeMainSigma) * (1 + SmartService.Downtime));
                yield return env.Timeout(Downtime);
                env.Log("Machine in Mainprocess repaired");
                ProcessControl.BrokenMain = false;
            }
            machine.Release(req);
            env.Log("{0} ProductNo {1}: Machine Mainprocess is finished", env.Now, product.ID);
        }

        
    }
}
