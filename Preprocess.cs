﻿using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    public class Preprocess
    {
        ProcessControl pc;
        Simulation Env;
        double DowntimePreMean;
        double DowntimePreSigma;
        TimeSpan ProductionTime;
        TimeSpan Downtime;
        Analysis analysis;
       
        public Preprocess(ProcessControl pc, Simulation env, double downtimepremean, double downtimepresigma, Analysis analysis)
        {
            this.pc = pc;
            Env = env;
            this.analysis = analysis;
            // fix
            // hier noch Parameter die eingelesen werden übergeben und setzten
            DowntimePreMean = downtimepremean;
            DowntimePreSigma = downtimepresigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product)
        {
            //Env.Log("{0} ProductNo {1}: Machine Preprocess is in production", Env.Now, product.ID);
            ProductionTime = Env.RandLogNormal2(product.ProductionTimePreMean, product.ProductionTimePreSigma);
            analysis.APTPre = analysis.APTPre.Add(ProductionTime);
            yield return Env.Timeout(ProductionTime);
            if (Env.ActiveProcess.HandleFault())
            {
                pc.BrokenPre = true;
                //Env.Log("Break Machine in Preprocess");
                // Ausfalldauer für M
                Downtime = Env.RandLogNormal2(TimeSpan.FromDays(DowntimePreMean), TimeSpan.FromDays(DowntimePreSigma));
                Console.WriteLine("PRE: "+Downtime.TotalHours);
                analysis.ADOTPre = analysis.ADOTPre.Add(Downtime);
                product.Broken = true;
                yield return Env.Timeout(Downtime);
                //Env.Log("Machine in Preprocess repaired");
                pc.BrokenPre = false;
            }
            machine.Release(req);
            //Env.Log("{0} ProductNo {1}: Machine Preprocess is finished", Env.Now, product.ID);
        }
    }
}
