using System;
using System.Collections.Generic;
using SimSharp;
using System.Text;

namespace ProduktionssystemSimulation
{
    static class ReviewRework
    {

        static double SpoiltRatio = 0.15;
        static double ReworkRatio = 0.1;

        public static (int,int) Review(Simulation env, int reworkQuantity, int reproductionQuantity)
        {
            env.Log("Review");
            // 0 bis 0.1: Nacharbeit
            // >0.1 bis 0.15: Ausschuss
            var reviewRatio = env.RandUniform(0, 1);
            if (reviewRatio > 0 && reviewRatio <= SpoiltRatio)
            {
                env.Log("Rework");
                reworkQuantity++;
                env.Log("Rework quantity: {0}", reworkQuantity);
            }
            else if (reviewRatio > SpoiltRatio && reviewRatio <= ReworkRatio)
            {
                env.Log("Spoilt");
                reproductionQuantity++;
                env.Log("Spoilt quantity: {0}", reproductionQuantity);
            }
            return (reworkQuantity, reproductionQuantity);
        }

        public static IEnumerable<Event> Rework(Simulation env, int reworkQuantity)
        {
            int i = 0;
            while (i < reworkQuantity)
            {
                env.Log("Rework of Product {0}", i + 1);
                yield return env.Timeout(TimeSpan.FromMinutes(10));
                i++;
            }
        }
    }
}
