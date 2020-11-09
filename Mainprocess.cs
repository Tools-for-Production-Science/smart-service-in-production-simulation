using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    /*
     * 
     * Hauptprozess.
     * Der Smart Service nimmt hier Einfluss.
     * 
     */

    class Mainprocess
    {
        readonly private ProcessControl pc;
        readonly private Simulation env;
        readonly private SmartService smartService;
        readonly private Analysis analysis;
        readonly private double downtimeMainMean;
        readonly private double downtimeMainSigma;

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
            TimeSpan ProductionTime = env.RandNormalPositive(product.ProductionTimeMainMean, product.ProductionTimeMainSigma);

            // Für die KPI berechnung, die gesamt Zeit in der die Maschine läuft abspeichern.
            analysis.MachineWorkingTimeMain = analysis.MachineWorkingTimeMain.Add(ProductionTime);

            yield return env.Timeout(ProductionTime);

            // Wenn ein Prozess unterbrochen wird, muss die Iteratormethode HandleFault () aufrufen, 
            // bevor weitere Ereignisse ausgegeben werden können.
            // Diese Methode muss aufgerufen werden, um das IsOk-Flag des Prozesses auf true zurückzusetzen.
            if (env.ActiveProcess.HandleFault())
            {
                pc.brokenMain = true;

                TimeSpan downtime = TimeSpan.FromDays(env.RandLogNormal2(downtimeMainMean* (1  - smartService.smartServiceEffectDowntimeMean),downtimeMainSigma * (1 - smartService.smartServiceEffectDowntimeSigma)) );
                // Ausfallzeit der Maschine für die Berechnung der KPI berechnen.
                analysis.DowntimeMain = analysis.DowntimeMain.Add(downtime);

                product.Broken = true;
                yield return env.Timeout(downtime);
                pc.brokenMain = false;
            }

            // Maschine wieder frei geben, sobald das Produkt fertig produziert ist.
            machine.Release(req);
        }

        
    }
}
