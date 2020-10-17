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
        TimeSpan ProductionTime;

        public Mainprocess(Simulation env, SmartService smartService, double downtimemainmean, double downtimemainsigma)
        {
            Env = env;
            SmartService = smartService;
            DowntimeMainMean = downtimemainmean;
            DowntimeMainSigma = downtimemainsigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product, Analysis analysis)
        {
            Env.Log("{0} ProductNo {1}: Machine Mainprocess is in production", Env.Now, product.ID);
            ProductionTime = Env.RandNormalPositive(product.ProductionTimeMainMean, product.ProductionTimeMainSigma);
            analysis.APTMain += ProductionTime;
            yield return Env.Timeout(ProductionTime);
            if (Env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenMain = true;
                Env.Log("Break Machine in Mainprocess");
                // Ausfalldauer für M
                Downtime = TimeSpan.FromDays(Env.RandLogNormal2(DowntimeMainMean,DowntimeMainSigma) * (1 + SmartService.Downtime));
                analysis.ADOTMain += Downtime;
                yield return Env.Timeout(Downtime);
                Env.Log("Machine in Mainprocess repaired");
                ProcessControl.BrokenMain = false;
            }
            machine.Release(req);
            Env.Log("{0} ProductNo {1}: Machine Mainprocess is finished", Env.Now, product.ID);
        }

        
    }
}
