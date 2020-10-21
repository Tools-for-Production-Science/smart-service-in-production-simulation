using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    public class Preprocess
    {
        readonly ProcessControl pc;
        readonly Simulation env;
        readonly Analysis analysis;
        readonly double downtimePreMean;
        readonly double downtimePreSigma;

        public Preprocess(ProcessControl pc, Simulation env, double downtimePreMean, double downtimePreSigma, Analysis analysis)
        {
            this.pc = pc;
            this.env = env;
            this.analysis = analysis;
            this.downtimePreMean = downtimePreMean;
            this.downtimePreSigma = downtimePreSigma;
        }

        public IEnumerable<Event> ProductionStep(Resource machine, Request req, Product product)
        {
            //Env.Log("{0} ProductNo {1}: Machine Preprocess is in production", Env.Now, product.ID);

            TimeSpan productionTime = env.RandLogNormal2(product.ProductionTimePreMean, product.ProductionTimePreSigma);

            // Für die KPI berechnung, die gesamt Zeit in der die Maschine läuft abspeichern.
            analysis.APTPre = analysis.APTPre.Add(productionTime);

            yield return env.Timeout(productionTime);

            // Wenn ein Prozess unterbrochen wird, muss die Iteratormethode HandleFault () aufrufen, 
            // bevor weitere Ereignisse ausgegeben werden können.
            // Diese Methode muss aufgerufen werden, um das IsOk-Flag des Prozesses auf true zurückzusetzen.
            if (env.ActiveProcess.HandleFault())
            {
                pc.BrokenPre = true;
                //Env.Log("Break Machine in Preprocess");

                TimeSpan downtime = env.RandLogNormal2(TimeSpan.FromDays(downtimePreMean), TimeSpan.FromDays(downtimePreSigma));

                //Console.WriteLine("PRE: "+Downtime.TotalHours);

                // Ausfallzeit der Maschine für die Berechnung der KPI berechnen.
                analysis.ADOTPre = analysis.ADOTPre.Add(downtime);

                product.Broken = true;

                yield return env.Timeout(downtime);
                //Env.Log("Machine in Preprocess repaired");
                pc.BrokenPre = false;
            }

            // Maschine wieder frei geben, sobald das Produkt fertig produziert ist.
            machine.Release(req);
            //Env.Log("{0} ProductNo {1}: Machine Preprocess is finished", Env.Now, product.ID);
        }
    }
}
