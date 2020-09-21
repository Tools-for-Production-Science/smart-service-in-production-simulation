﻿using System;
using System.Collections.Generic;
using SimSharp;
using System.Text;

namespace ProduktionssystemSimulation
{
    static class ReviewRework
    {
        public static IEnumerable<Event> ReviewPre(Simulation env, Position position, Product product)
        {
            env.Log("Review after Preprocess");
            
            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio < (1-(position.ScrapPreMean+position.ReworkPreMean)))
            {
               env.Log("End of review. Product {0} corresponds to the quality", position.ID);
            }
            else if ((1 - (position.ScrapPreMean + position.ReworkPreMean))<= reviewRatio && reviewRatio < (1-position.ScrapPreMean))
            {
                env.Log("Rework: Product {0}", position.ID);
                yield return env.Process(Rework(env, product));
            } else
            { 
                env.Log("Scrap: Product {0}", position.ID);
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> ReviewMain(Simulation env, Position position, Product product, SmartService smartService)
        {
            env.Log("Review after Mainprocess");

            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio < (1 - ((position.ScrapMainMean * (1+ smartService.Scrap))+ (position.ReworkMainMean * (1+smartService.Rework)))))
            {
                env.Log("End of review. Product {0} corresponds to the quality", position.ID);
            }
            else if ((1 - ((position.ScrapMainMean * (1 + smartService.Scrap)) + (position.ReworkMainMean * (1 + smartService.Rework)))) <= reviewRatio && reviewRatio < (1 - (position.ScrapMainMean * (1 + smartService.Scrap))))
            {
                env.Log("Rework: Product {0}", position.ID);
                yield return env.Process(Rework(env, product));
            }
            else
            {
                env.Log("Scrap: Product {0}", position.ID);
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> ReviewPost(Simulation env, Position position, Product product)
        {
            env.Log("Review after Postprocess");

            var reviewRatio = env.RandUniform(0, 1);

            if (0 <= reviewRatio && reviewRatio < (1 - (position.ScrapPostMean + position.ReworkPostMean)))
            {
                env.Log("End of review. Product {0} corresponds to the quality", position.ID);
            }
            else if ((1 - (position.ScrapPostMean + position.ReworkPostMean)) <= reviewRatio && reviewRatio < (1 - position.ScrapPostMean))
            {
                env.Log("Rework: Product {0}", position.ID);
                yield return env.Process(Rework(env, product));
            }
            else
            {
                env.Log("Scrap: Product {0}", position.ID);
                product.Broken = true;
            }
        }

        public static IEnumerable<Event> Rework(Simulation env, Product product)
        { 
            env.Log("Rework of Product {0}", product.ID);
            yield return env.TimeoutNormalPositive(product.ReworkTimeMean, product.ReworkTimeSigma);
            env.Log("Rework of Product {0} done", product.ID);
        }
    }
}
