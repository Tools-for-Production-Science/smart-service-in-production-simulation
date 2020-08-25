using System;
using System.Collections.Generic;
using SimSharp;
using System.Text;

namespace ProduktionssystemSimulation
{
    static class ReviewRework
    {

        static double ScrapRatioPre = 0.15;
        static double ReworkRatioPre = 0.1;
        static double ScrapRatioMain = 0.15;
        static double ReworkRatioMain = 0.1;
        static double ScrapRatioPost = 0.15;
        static double ReworkRatioPost = 0.1;

        public static IEnumerable<Event> ReviewPre(Simulation env, Product product)
        {
            env.Log("Review after Preprocess");
            ScrapRatioPre = env.RandNormalPositive(product.ScrapPreMean, product.ScrapPreSigma);
            // 0 bis ScrapRatio: Scrap
            // >ScrapRatio bis ReworkRatio+ScrapRatio: Rework
            var reviewRatio = env.RandUniform(0, 1);
            if (reviewRatio > 0 && reviewRatio <= ScrapRatioPre)
            {
                env.Log("Scrap: Product {0}", product.ID);
                product.Broken = true;
               
            }
            else if (reviewRatio > ScrapRatioPre && reviewRatio <= ReworkRatioPre+ScrapRatioPre)
            {
                env.Log("Rework: Product {0}", product.ID);
                yield return env.Process(Rework(env, product));
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
            if (reviewRatio > 0 && reviewRatio <= ScrapRatioMain)
            {
                env.Log("Scrap: Product {0}", product.ID);
                product.Broken = true;
                
            }
            else if (reviewRatio > ScrapRatioMain && reviewRatio <= ReworkRatioMain+ScrapRatioMain)
            {
                env.Log("Rework: Product {0}", product.ID);
                yield return env.Process(Rework(env, product));
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
            if (reviewRatio > 0 && reviewRatio <= ScrapRatioPost)
            {
                env.Log("Scrap: Product {0}", product.ID);
                product.Broken = true;
                
            }
            else if (reviewRatio > ScrapRatioPost && reviewRatio <= ReworkRatioPost+ScrapRatioPost)
            {
                env.Log("Rework: Product {0}", product.ID);
                yield return env.Process(Rework(env, product));
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
