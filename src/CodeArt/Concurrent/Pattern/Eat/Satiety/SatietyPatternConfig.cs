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
    public sealed class SatietyPatternConfig : EatPatternConfig
    {

        public SatietyPatternConfig(int maxFoodStock, int maxNutrientStock, CompoundReplaceMode replaceMode)
            : base(maxFoodStock, maxNutrientStock, replaceMode)
        {
        }
    }
}
