using System;
using System.Collections.Generic;
using SimSharp;
using System.Text;

namespace ProductionsystemSimulation
{
    /*
     * 
     * This class makes a separate check for each process step
     * 
     */
    static class ReviewRework
    {
        public static IEnumerable<Event> ReviewPre(Simulation env, Producttype producttype, Product product, Analysis analysis)
        {
            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio <= (1-(producttype.ScrapPreMean+producttype.ReworkPreMean)))
            {
                analysis.QuantityOfGoodPre = analysis.QuantityOfGoodPre + 1;
            }
            else if ((1 - (producttype.ScrapPreMean + producttype.ReworkPreMean))< reviewRatio && reviewRatio < (1-producttype.ScrapPreMean))
            {
                analysis.QuantityOfReworkPre = analysis.QuantityOfReworkPre + 1;
                yield return env.Process(Rework(env, product));
            } else
            { 
                analysis.QuantityOfScrapPre = analysis.QuantityOfScrapPre + 1;
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> ReviewMain(Simulation env, Producttype producttype, Product product, SmartService smartService, Analysis analysis)
        {
            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio <= (1 - ((producttype.ScrapMainMean * (1 - smartService.smartServiceEffectScrap))+ (producttype.ReworkMainMean * (1 - smartService.smartServiceEffectRework)))))
            {
                analysis.QuantityOfGoodMain = analysis.QuantityOfGoodMain + 1;
            }
            else if ((1 - ((producttype.ScrapMainMean * (1 + smartService.smartServiceEffectScrap)) + (producttype.ReworkMainMean * (1 - smartService.smartServiceEffectRework)))) < reviewRatio && reviewRatio < (1 - (producttype.ScrapMainMean * (1 - smartService.smartServiceEffectScrap))))
            {
                analysis.QuantityOfReworkMain = analysis.QuantityOfReworkMain + 1;
                yield return env.Process(Rework(env, product));
            }
            else
            {
                analysis.QuantityOfScrapMain = analysis.QuantityOfScrapMain + 1;
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> ReviewPost(Simulation env, Producttype producttype, Product product, Analysis analysis)
        {
            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio <= (1 - (producttype.ScrapPostMean + producttype.ReworkPostMean)))
            {
                analysis.QuantityOfGoodPost = analysis.QuantityOfGoodPost + 1;
            }
            else if ((1 - (producttype.ScrapPostMean + producttype.ReworkPostMean)) < reviewRatio && reviewRatio < (1 - producttype.ScrapPostMean))
            {
                analysis.QuantityOfReworkPost = analysis.QuantityOfReworkPost + 1;
                yield return env.Process(Rework(env, product));
            }
            else
            {
                analysis.QuantityOfScrapPost = analysis.QuantityOfScrapPost +1;
                product.Broken = true;
            }
        }

        // Performs the rework
        public static IEnumerable<Event> Rework(Simulation env, Product product)
        { 
            TimeSpan reworkTime = env.RandLogNormal2(product.ReworkTimeMean, product.ReworkTimeSigma);
            product.TotalReworkTime = reworkTime;
            yield return env.Timeout(reworkTime);
        }
    }
}
