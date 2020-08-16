﻿using System;
using System.Collections.Generic;
using SimSharp;

namespace ProduktionssystemSimulation
{
    class Mainprocess
    {
        Simulation Env;
        TimeSpan RepairTime;

        public Mainprocess(Simulation env, SmartService smartService)
        {
            Env = env;
            RepairTime = TimeSpan.FromMinutes(30 * (1 - smartService.Effektausmaß * smartService.MaschinenAusfallwkeit));
        }

        public IEnumerable<Event> ProductionStep(Simulation env, Resource machine, Request req, int id, TimeSpan productionTime)
        {
            env.Log("{0} ProductNo {1}: Machine Mainprocess is in production", env.Now, id);

            yield return env.Timeout(productionTime);
            if (env.ActiveProcess.HandleFault())
            {
                ProcessControl.BrokenMain = true;
                env.Log("Break Machine in Mainprocess");
                // Ausfalldauer für M
                yield return env.Timeout(RepairTime);
                env.Log("Machine in Mainprocess repaired");
                ProcessControl.BrokenMain = false;
            }
            machine.Release(req);
            env.Log("{0} ProductNo {1}: Machine Mainprocess is finished", env.Now, id);
        }

        
    }
}