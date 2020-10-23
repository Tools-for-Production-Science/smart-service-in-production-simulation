using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Postprocess
    {
        readonly ProcessControl pc;
        readonly Simulation env;
        readonly Analysis analysis;
        readonly double downtimePostMean;
        readonly double downtimePostSigma;

        public Postprocess(ProcessControl pc, Simulation env, double downtimePostMean, double downtimePostSigma, Analysis analysis)
        {
            this.pc = pc;
            this.analysis = analysis;
            this.env = env;
            this.downtimePostMean = downtimePostMean;
            this.downtimePostSigma = downtimePostSigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product)
        {
            //Env.Log("{0} ProductNo {1}: Machine Postprocess is in production", Env.Now, product.ID);
            TimeSpan ProductionTime = env.RandLogNormal2(product.ProductionTimePostMean, product.ProductionTimePostSigma);

            // Für die KPI berechnung, die gesamt Zeit in der die Maschine läuft abspeichern.
            analysis.APTPost = analysis.APTPost.Add(ProductionTime);

            yield return env.Timeout(ProductionTime);

            // Wenn ein Prozess unterbrochen wird, muss die Iteratormethode HandleFault () aufrufen, 
            // bevor weitere Ereignisse ausgegeben werden können.
            // Diese Methode muss aufgerufen werden, um das IsOk-Flag des Prozesses auf true zurückzusetzen.
            if (env.ActiveProcess.HandleFault())
            {
                pc.brokenPost = true;
                //Env.Log("Break Machine Postprocess");

                TimeSpan downtime = env.RandLogNormal2(TimeSpan.FromDays(downtimePostMean), TimeSpan.FromDays(downtimePostSigma));

                // Ausfallzeit der Maschine für die Berechnung der KPI berechnen.
                analysis.ADOTPost = analysis.ADOTPost.Add(downtime);
                product.Broken = true;
                yield return env.Timeout(downtime);
                //Env.Log("Machine in Postprocess repaired");
                pc.brokenPost = false;
            }
            // Maschine wieder frei geben, sobald das Produkt fertig produziert ist.
            machine.Release(req);
            //Env.Log("{0} ProductNo {1}: Machine Postprocess is finished", Env.Now, product.ID);
        }

       
       
    }
}
