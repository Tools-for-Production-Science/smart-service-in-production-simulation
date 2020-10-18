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
        Analysis analysis;

        public Mainprocess(ProcessControl pc, Simulation env, SmartService smartService, double downtimemainmean, double downtimemainsigma, Analysis analysis)
        {
            this.pc = pc;
            Env = env;
            SmartService = smartService;
            DowntimeMainMean = downtimemainmean;
            DowntimeMainSigma = downtimemainsigma;
            this.analysis = analysis;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product)
        {
            //Env.Log("{0} ProductNo {1}: Machine Mainprocess is in production", Env.Now, product.ID);
            ProductionTime = Env.RandNormalPositive(product.ProductionTimeMainMean, product.ProductionTimeMainSigma);
            analysis.APTMain = analysis.APTMain.Add(ProductionTime);
            yield return Env.Timeout(ProductionTime);
            if (Env.ActiveProcess.HandleFault())
            {
                pc.BrokenMain = true;
                //Env.Log("Break Machine in Mainprocess");
                // Ausfalldauer für M
                Downtime = TimeSpan.FromDays(Env.RandLogNormal2(DowntimeMainMean* (1  - SmartService.DowntimeMean),DowntimeMainSigma * (1 - SmartService.DowntimeSigma)) );

                //Console.WriteLine("Davor: " + analysis.ADOTMain.TotalMinutes);
                analysis.ADOTMain = analysis.ADOTMain.Add(Downtime);
                //Console.WriteLine("Danach: " + analysis.ADOTMain.TotalMinutes);
                //Console.WriteLine("MAIN: "+Downtime.TotalMinutes);
                product.Broken = true;
                yield return Env.Timeout(Downtime);
                //Env.Log("Machine in Mainprocess repaired");
                pc.BrokenMain = false;
            }
            machine.Release(req);
            //Env.Log("{0} ProductNo {1}: Machine Mainprocess is finished", Env.Now, product.ID);
        }

        
    }
}
