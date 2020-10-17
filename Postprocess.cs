using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Postprocess
    {
        ProcessControl pc;
        Simulation Env;
        double DowntimePostMean;
        double DowntimePostSigma;
        TimeSpan ProductionTime;
        TimeSpan Downtime;

        public Postprocess(ProcessControl pc, Simulation env, double downtimepostmean, double downtimepostsigma)
        {
            this.pc = pc;
            Env = env;
            DowntimePostMean = downtimepostmean;
            DowntimePostSigma = downtimepostsigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product, Analysis analysis)
        {
            Env.Log("{0} ProductNo {1}: Machine Postprocess is in production", Env.Now, product.ID);
            ProductionTime = Env.RandNormalPositive(product.ProductionTimePostMean, product.ProductionTimePostSigma);
            analysis.APTPost = analysis.APTPost.Add(ProductionTime);
            yield return Env.Timeout(ProductionTime);
            if (Env.ActiveProcess.HandleFault())
            {
                pc.BrokenPost = true;
                Env.Log("Break Machine Postprocess");
                // Ausfalldauer für M
                Downtime = Env.RandLogNormal2(TimeSpan.FromDays(DowntimePostMean), TimeSpan.FromDays(DowntimePostSigma));
                analysis.ADOTPost = analysis.ADOTPost.Add(Downtime);
                yield return Env.Timeout(Downtime);
                Env.Log("Machine in Postprocess repaired");
                pc.BrokenPost = false;
            }
            machine.Release(req);
            Env.Log("{0} ProductNo {1}: Machine Postprocess is finished", Env.Now, product.ID);
        }

       
       
    }
}
