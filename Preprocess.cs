using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    public class Preprocess
    {
        readonly private ProcessControl pc;
        readonly private Simulation env;
        readonly private Analysis analysis;
        readonly private double downtimePreMean;
        readonly private double downtimePreSigma;

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
            TimeSpan productionTime = env.RandLogNormal2(product.ProductionTimePreMean, product.ProductionTimePreSigma);

            // Für die KPI berechnung, die gesamt Zeit in der die Maschine läuft abspeichern.
            analysis.MachineWorkingTimePre = analysis.MachineWorkingTimePre.Add(productionTime);

            yield return env.Timeout(productionTime);

            // Wenn ein Prozess unterbrochen wird, muss die Iteratormethode HandleFault () aufrufen, 
            // bevor weitere Ereignisse ausgegeben werden können.
            // Diese Methode muss aufgerufen werden, um das IsOk-Flag des Prozesses auf true zurückzusetzen.
            if (env.ActiveProcess.HandleFault())
            {
                pc.brokenPre = true;

                TimeSpan downtime = env.RandLogNormal2(TimeSpan.FromDays(downtimePreMean), TimeSpan.FromDays(downtimePreSigma));

                // Ausfallzeit der Maschine für die Berechnung der KPI berechnen.
                analysis.DowntimePre = analysis.DowntimePre.Add(downtime);

                product.Broken = true;

                yield return env.Timeout(downtime);
                pc.brokenPre = false;
            }

            // Maschine wieder frei geben, sobald das Produkt fertig produziert ist.
            machine.Release(req);
        }
    }
}
