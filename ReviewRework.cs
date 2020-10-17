using System;
using System.Collections.Generic;
using SimSharp;
using System.Text;

namespace ProduktionssystemSimulation
{
    static class ReviewRework
    {
        public static IEnumerable<Event> ReviewPre(Simulation env, Position position, Product product, Analysis analysis)
        {
            env.Log("Review after Preprocess");
            
            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio <= (1-(position.ScrapPreMean+position.ReworkPreMean)))
            {
                analysis.QuantityOfGoodPre = analysis.QuantityOfGoodPre+1;
                env.Log("End of review. Product {0} corresponds to the quality", position.ID);
            }
            else if ((1 - (position.ScrapPreMean + position.ReworkPreMean))< reviewRatio && reviewRatio < (1-position.ScrapPreMean))
            {
                env.Log("Rework: Product {0}", position.ID);
                analysis.QuantityOfReworkPre = analysis.QuantityOfReworkPre + 1;
                yield return env.Process(Rework(env, product));
            } else
            { 
                env.Log("Scrap: Product {0}", position.ID);
                analysis.QuantityOfScrapPre = analysis.QuantityOfScrapPre +1;
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> ReviewMain(Simulation env, Position position, Product product, SmartService smartService, Analysis analysis)
        {
            env.Log("Review after Mainprocess");

            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio <= (1 - ((position.ScrapMainMean * (1 - smartService.Scrap))+ (position.ReworkMainMean * (1 - smartService.Rework)))))
            {
                analysis.QuantityOfReworkMain = analysis.QuantityOfReworkMain +1;
                env.Log("End of review. Product {0} corresponds to the quality", position.ID);
            }
            else if ((1 - ((position.ScrapMainMean * (1 + smartService.Scrap)) + (position.ReworkMainMean * (1 - smartService.Rework)))) < reviewRatio && reviewRatio < (1 - (position.ScrapMainMean * (1 - smartService.Scrap))))
            {
                env.Log("Rework: Product {0}", position.ID);
                analysis.QuantityOfReworkMain = analysis.QuantityOfReworkMain +1;
                yield return env.Process(Rework(env, product));
            }
            else
            {
                env.Log("Scrap: Product {0}", position.ID);
                analysis.QuantityOfScrapMain = analysis.QuantityOfScrapMain +1;
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> ReviewPost(Simulation env, Position position, Product product, Analysis analysis)
        {
            env.Log("Review after Postprocess");

            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio <= (1 - (position.ScrapPostMean + position.ReworkPostMean)))
            {
                analysis.QuantityOfGoodPost = analysis.QuantityOfGoodPost +1;
                env.Log("End of review. Product {0} corresponds to the quality", position.ID);
            }
            else if ((1 - (position.ScrapPostMean + position.ReworkPostMean)) < reviewRatio && reviewRatio < (1 - position.ScrapPostMean))
            {
                env.Log("Rework: Product {0}", position.ID);
                analysis.QuantityOfReworkPost = analysis.QuantityOfReworkPost +1;
                yield return env.Process(Rework(env, product));
            }
            else
            {
                env.Log("Scrap: Product {0}", position.ID);
                analysis.QuantityOfScrapPre = analysis.QuantityOfScrapPre +1;
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> Rework(Simulation env, Product product)
        { 
            env.Log("Rework of Product {0}", product.ID);
            TimeSpan reworkTime = env.RandLogNormal2(product.ReworkTimeMean, product.ReworkTimeSigma);
            product.TotalReworkTime = reworkTime;
            yield return env.Timeout(reworkTime);
            env.Log("Rework of Product {0} done", product.ID);
        }
    }
}
