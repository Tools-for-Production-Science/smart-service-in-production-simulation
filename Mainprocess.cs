using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Mainprocess
    {
        ProcessControl pc;
        Simulation Env;
        TimeSpan Downtime;
        SmartService SmartService;
        double DowntimeMainMean;
        double DowntimeMainSigma;
        TimeSpan ProductionTime;

        public Mainprocess(ProcessControl pc, Simulation env, SmartService smartService, double downtimemainmean, double downtimemainsigma)
        {
            this.pc = pc;
            Env = env;
            SmartService = smartService;
            DowntimeMainMean = downtimemainmean;
            DowntimeMainSigma = downtimemainsigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product, Analysis analysis)
        {
            Env.Log("{0} ProductNo {1}: Machine Mainprocess is in production", Env.Now, product.ID);
            ProductionTime = Env.RandNormalPositive(product.ProductionTimeMainMean, product.ProductionTimeMainSigma);
            analysis.APTMain = analysis.APTMain.Add(ProductionTime);
            yield return Env.Timeout(ProductionTime);
            if (Env.ActiveProcess.HandleFault())
            {
                pc.BrokenMain = true;
                Env.Log("Break Machine in Mainprocess");
                // Ausfalldauer für M
                Downtime = TimeSpan.FromDays(Env.RandLogNormal2(DowntimeMainMean,DowntimeMainSigma) * (1  - SmartService.Downtime));
                analysis.ADOTMain = analysis.ADOTMain.Add(Downtime);
                yield return Env.Timeout(Downtime);
                Env.Log("Machine in Mainprocess repaired");
                pc.BrokenMain = false;
            }
            machine.Release(req);
            Env.Log("{0} ProductNo {1}: Machine Mainprocess is finished", Env.Now, product.ID);
        }

        
    }
}
