using System;
using System.Collections.Generic;
using SimSharp;

namespace ProductionsystemSimulation
{
    /*
     * 
     * Mainprocess
     * The Smart Service takes influence here
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

            // For the KPI calculation, save the total time the machine is running
            analysis.MachineWorkingTimeMain = analysis.MachineWorkingTimeMain.Add(ProductionTime);

            yield return env.Timeout(ProductionTime);

            // When a process is interrupted, the HandleFault() iterator method must be called before further events can be issued
            // This method must be called to reset the IsOk flag of the process to true
            if (env.ActiveProcess.HandleFault())
            {
                pc.brokenMain = true;

                TimeSpan downtime = TimeSpan.FromDays(env.RandLogNormal2(downtimeMainMean* (1  - smartService.smartServiceEffectDowntimeMean),downtimeMainSigma * (1 - smartService.smartServiceEffectDowntimeSigma)) );
                // Calculate downtime of the machine for the calculation of the KPI
                analysis.DowntimeMain = analysis.DowntimeMain.Add(downtime);

                product.Broken = true;
                yield return env.Timeout(downtime);
                pc.brokenMain = false;
            }

            // Release the machine again as soon as the product has been produced
            machine.Release(req);
        }

        
    }
}
