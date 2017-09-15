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


using CodeArt.IO;


namespace CodeArt.Concurrent.Pattern
{
    public class EatPatternConfig
    {
        /// <summary>
        /// 最大食物储备量，吃的食物大于该值，那么不再吃新的食物，或者抛弃旧的食物而接受新的食物
        /// </summary>
        public int MaxFoodStock { get; private set; }


        /// <summary>
        /// 替换模式
        /// </summary>
        public CompoundReplaceMode ReplaceMode { get; private set; }

        /// <summary>
        /// 养分的最大储备量，养分数量大于该值，就会抛弃养分
        /// </summary>
        public int MaxNutrientStock { get; private set; }


        public EatPatternConfig(int maxFoodStock, int maxNutrientStock, CompoundReplaceMode replaceMode)
        {
            this.MaxFoodStock = maxFoodStock;
            this.MaxNutrientStock = maxNutrientStock;
            this.ReplaceMode = replaceMode;
        }
    }
}
