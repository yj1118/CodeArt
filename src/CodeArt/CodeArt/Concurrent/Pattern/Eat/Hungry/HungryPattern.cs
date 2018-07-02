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

using System.Threading;
using CodeArt.IO;
using CodeArt.Concurrent.Pattern.Eat;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 饥渴模式，胃可以吃食物，吃的食物需要被消化，被酶分解成养分，每得到一个养分，就输出使用
    /// 食物会被不停的消化成酶，养分也会被不停的输出
    /// 未处理的食物会被取出给酶分解成养分
    /// </summary>
    public class HungryPattern<F, N> : EatPattern<F, N>
    {
        private readonly int _nutrientOutputInterval;

        protected HungryPattern(BreakDownHandler howBreakDown, BreakDownsHandler howBreakDowns, HungryPatternConfig config)
            : base(howBreakDown, howBreakDowns, config)
        {
            _nutrientOutputInterval = config.NutrientOutputInterval;
        }

        protected override int GetNutrientOutputInterval()
        {
            return _nutrientOutputInterval;
        }
    }

    /// <summary>
    /// 饥饿模式，1个食物分解1个养分
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="N"></typeparam>
    public class HungrySingle<F, N> : HungryPattern<F, N>
    {
        public HungrySingle(BreakDownHandler howBreakDown, HungryPatternConfig config)
            : base(howBreakDown, null, config)
        {
        }
    }

    /// <summary>
    /// 饥饿模式，1个食物分解多个养分
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="N"></typeparam>
    public class HungryMultiple<F, N> : HungryPattern<F, N>
    {
        public HungryMultiple(BreakDownsHandler howBreakDowns, HungryPatternConfig config)
            : base(null, howBreakDowns, config)
        {
        }
    }
}
