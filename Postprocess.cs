using System;
using System.Collections.Generic;
using SimSharp;

namespace ProductionsystemSimulation
{
    /*
     * 
     * This class represents the post-process
     * 
     */
    class Postprocess
    {
        readonly private ProcessControl pc;
        readonly private Simulation env;
        readonly private Analysis analysis;
        readonly private double downtimePostMean;
        readonly private double downtimePostSigma;

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
            TimeSpan ProductionTime = env.RandLogNormal2(product.ProductionTimePostMean, product.ProductionTimePostSigma);

            // For the KPI calculation, save the total time the machine is running
            analysis.MachineWorkingTimePost = analysis.MachineWorkingTimePost.Add(ProductionTime);

            yield return env.Timeout(ProductionTime);

            // When a process is interrupted, the HandleFault () iterator method must be called before further events can be emitted
            // This method must be called to reset the IsOk flag of the process to true
            if (env.ActiveProcess.HandleFault())
            {
                pc.brokenPost = true;

                TimeSpan downtime = env.RandLogNormal2(TimeSpan.FromDays(downtimePostMean), TimeSpan.FromDays(downtimePostSigma));

                // Calculate downtime of the machine for the calculation of the KPI
                analysis.DowntimePost = analysis.DowntimePost.Add(downtime);
                product.Broken = true;
                yield return env.Timeout(downtime);
                pc.brokenPost = false;
            }

            // Release the machine again as soon as the product has been produced
            machine.Release(req);
        }

       
       
       
    }
}
