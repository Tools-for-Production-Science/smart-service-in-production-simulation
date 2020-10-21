using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Mainprocess
    {
        readonly ProcessControl pc;
        readonly Simulation env;
        readonly SmartService smartService;
        readonly Analysis analysis;
        readonly double downtimeMainMean;
        readonly double downtimeMainSigma;

        public Mainprocess(ProcessControl pc, Simulation env, SmartService smartService, double downtimeMainMean, double downtimeMainSigma, Analysis analysis)
        {
            this.pc = pc;
            this.env = env;
            this.smartService = smartService;
            this.downtimeMainMean = downtimeMainMean;
            this.downtimeMainSigma = downtimeMainSigma;
            this.analysis = analysis;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product)
        {
            //Env.Log("{0} ProductNo {1}: Machine Mainprocess is in production", Env.Now, product.ID);
            TimeSpan ProductionTime = env.RandNormalPositive(product.ProductionTimeMainMean, product.ProductionTimeMainSigma);

            // Für die KPI berechnung, die gesamt Zeit in der die Maschine läuft abspeichern.
            analysis.APTMain = analysis.APTMain.Add(ProductionTime);

            yield return env.Timeout(ProductionTime);

            // Wenn ein Prozess unterbrochen wird, muss die Iteratormethode HandleFault () aufrufen, 
            // bevor weitere Ereignisse ausgegeben werden können.
            // Diese Methode muss aufgerufen werden, um das IsOk-Flag des Prozesses auf true zurückzusetzen.
            if (env.ActiveProcess.HandleFault())
            {
                pc.BrokenMain = true;
                //Env.Log("Break Machine in Mainprocess");

                TimeSpan downtime = TimeSpan.FromDays(env.RandLogNormal2(downtimeMainMean* (1  - smartService.DowntimeMean),downtimeMainSigma * (1 - smartService.DowntimeSigma)) );

                //Console.WriteLine("Davor: " + analysis.ADOTMain.TotalMinutes);

                // Ausfallzeit der Maschine für die Berechnung der KPI berechnen.
                analysis.ADOTMain = analysis.ADOTMain.Add(downtime);

                product.Broken = true;
                yield return env.Timeout(downtime);
                //Env.Log("Machine in Mainprocess repaired");
                pc.BrokenMain = false;
            }

            // Maschine wieder frei geben, sobald das Produkt fertig produziert ist.
            machine.Release(req);
            //Env.Log("{0} ProductNo {1}: Machine Mainprocess is finished", Env.Now, product.ID);
        }

        
    }
}
