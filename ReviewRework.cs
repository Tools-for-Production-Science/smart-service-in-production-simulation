using System;
using System.Collections.Generic;
using SimSharp;
using System.Text;

namespace ProduktionssystemSimulation
{
    static class ReviewRework
    {

        static double SpoiltRatioPre = 0.15;
        static double ReworkRatioPre = 0.1;
        static double SpoiltRatioMain = 0.15;
        static double ReworkRatioMain = 0.1;
        static double SpoiltRatioPost = 0.15;
        static double ReworkRatioPost = 0.1;

        public static IEnumerable<Event> ReviewPre(Simulation env, Product product)
        {
            env.Log("Review after Preprocess");
            // 0 bis 0.1: Nacharbeit
            // >0.1 bis 0.15: Ausschuss
            var reviewRatio = env.RandUniform(0, 1);
            if (reviewRatio > 0 && reviewRatio <= SpoiltRatioPre)
            {
                env.Log("Rework: Product {0}", product.ID);
                yield return env.Process(Rework(env, product));
            }
            else if (reviewRatio > SpoiltRatioPre && reviewRatio <= ReworkRatioPre)
            {
                env.Log("Spoilt: Product {0}", product.ID);
                product.Broken = true;
            } else
            {
                env.Log("End of review. Product {0} corresponds to the quality", product.ID);
            }
        }

        public static IEnumerable<Event> ReviewMain(Simulation env, Product product)
        {
            env.Log("Review after Mainprocess");
            // 0 bis 0.1: Nacharbeit
            // >0.1 bis 0.15: Ausschuss
            var reviewRatio = env.RandUniform(0, 1);
            if (reviewRatio > 0 && reviewRatio <= SpoiltRatioMain)
            {
                env.Log("Rework: Product {0}", product.ID);
                yield return env.Process(Rework(env, product));
            }
            else if (reviewRatio > SpoiltRatioMain && reviewRatio <= ReworkRatioMain)
            {
                env.Log("Spoilt: Product {0}", product.ID);
                product.Broken = true;
            }
            else
            {
                env.Log("End of review. Product {0} corresponds to the quality", product.ID);
            }
        }

        public static IEnumerable<Event> ReviewPost(Simulation env, Product product)
        {
            env.Log("Review after Postprocess");
            // 0 bis 0.1: Nacharbeit
            // >0.1 bis 0.15: Ausschuss
            var reviewRatio = env.RandUniform(0, 1);
            if (reviewRatio > 0 && reviewRatio <= SpoiltRatioPost)
            {
                env.Log("Rework: Product {0}", product.ID);
                yield return env.Process(Rework(env, product));
            }
            else if (reviewRatio > SpoiltRatioPost && reviewRatio <= ReworkRatioPost)
            {
                env.Log("Spoilt: Product {0}", product.ID);
                product.Broken = true;
            }
            else
            {
                env.Log("End of review. Product {0} corresponds to the quality", product.ID);
            }
        }

        public static IEnumerable<Event> Rework(Simulation env, Product product)
        { 
            env.Log("Rework of Product {0}", product.ID);
            yield return env.Timeout(TimeSpan.FromMinutes(10));
            env.Log("Rework of Product {0} done", product.ID);
        }
    }
}
