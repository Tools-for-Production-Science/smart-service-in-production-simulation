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
        TimeSpan ProductionTime;
        TimeSpan Downtime;
       
        public Preprocess(Simulation env, double downtimepremean, double downtimepresigma)
        {
            Env = env;
            // fix
            // hier noch Parameter die eingelesen werden übergeben und setzten
            DowntimePreMean = downtimepremean;
            DowntimePreSigma = downtimepresigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product, Analysis analysis)
        {
            Env.Log("{0} ProductNo {1}: Machine Preprocess is in production", Env.Now, product.ID);
            ProductionTime = Env.RandNormalPositive(product.ProductionTimePreMean, product.ProductionTimePreSigma);
            analysis.APTPre = analysis.APTPre.Add(ProductionTime);
            yield return Env.Timeout(ProductionTime);
            if (Env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenPre = true;
                Env.Log("Break Machine in Preprocess");
                // Ausfalldauer für M
                Downtime = Env.RandLogNormal2(TimeSpan.FromDays(DowntimePreMean), TimeSpan.FromDays(DowntimePreSigma));
                analysis.ADOTPre = analysis.ADOTPre.Add(Downtime);
                yield return Env.Timeout(Downtime);
                Env.Log("Machine in Preprocess repaired");
                ProcessControl.BrokenPre = false;
            }
            machine.Release(req);
            Env.Log("{0} ProductNo {1}: Machine Preprocess is finished", Env.Now, product.ID);
        }
    }
}
