using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;


namespace CodeArt.Concurrent.Pattern
{
    public sealed class HungryPatternConfig : EatPatternConfig
    {
        /// <summary>
        /// 输出养分的时间间隔，毫秒，0表示不停的输出，无间隔，注意，我们会把消化的时间算到养分时间间隔以内，例如100的意思是，每隔100毫秒就可以输出一次养分，分解时间也算在内
        /// </summary>
        public int NutrientOutputInterval { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxFoodStock">最大食物储备量，吃的食物大于该值，那么不再吃新的食物，或者抛弃旧的食物而接受新的食物</param>
        /// <param name="replaceMode">最大养分储备量，储备的养分如果大于该值，那么不再接受新的养分，或者抛弃旧的养分而接受新的养分</param>
        /// <param name="replaceMode">替换模式</param>
        /// <param name="nutrientOutputInterval">输出养分的时间间隔，毫秒，0表示不停的输出，无间隔，注意，我们会把消化的时间算到养分时间间隔以内，例如100的意思是，每隔100毫秒就可以输出一次养分，分解时间也算在内</param>
        public HungryPatternConfig(int maxFoodStock, int maxNutrientStock, CompoundReplaceMode replaceMode, int nutrientOutputInterval)
            : base(maxFoodStock, maxNutrientStock, replaceMode)
        {
            this.NutrientOutputInterval = nutrientOutputInterval;
        }
    }

}
