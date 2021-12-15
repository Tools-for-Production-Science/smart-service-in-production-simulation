using System;
using System.Collections.Generic;
using SimSharp;

namespace ProductionsystemSimulation
{
    /*
     * 
     * This class represents the pre-pore process
     * 
     */
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

            // For the KPI calculation, save the total time the machine is running
            analysis.MachineWorkingTimePre = analysis.MachineWorkingTimePre.Add(productionTime);

            yield return env.Timeout(productionTime);

            // When a process is interrupted, the HandleFault () iterator method must be called before further events can be emitted
            // This method must be called to reset the IsOk flag of the process to true
            if (env.ActiveProcess.HandleFault())
            {
                pc.brokenPre = true;

                TimeSpan downtime = env.RandLogNormal2(TimeSpan.FromDays(downtimePreMean), TimeSpan.FromDays(downtimePreSigma));

                // Calculate downtime of the machine for the calculation of the KPI
                analysis.DowntimePre = analysis.DowntimePre.Add(downtime);

                product.Broken = true;

                yield return env.Timeout(downtime);
                pc.brokenPre = false;
            }

            // Release the machine again as soon as the product has been produced
            machine.Release(req);
        }
    }
}
