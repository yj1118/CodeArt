using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

using CodeArt.IO;
using CodeArt.Concurrent.Pattern.Eat;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 请保证消化食物获取养分的执行时间要低于吃食物的时间间隔，否则无法保证温饱
    /// 请保证输出养分，使用养分的执行时间要低于吃食物的时间间隔，否则无法保证温饱
    /// 终止，消化食物获取养分和输出养分并使用，这两个行为的运行时间要低，越低越好
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="N"></typeparam>
    public class SatietyPattern<F, N> : EatPattern<F, N>
    {
        protected SatietyPattern(BreakDownHandler howBreakDown, BreakDownsHandler howBreakDowns, SatietyPatternConfig config)
            : base(howBreakDown, howBreakDowns, config)
        {
            InitInputInterval(); //初始化输入养分的时间间隔
            InitOutputElapsed();  //初始化输出养分所运行的时间
        }

        private object _syncObject = new object();

        protected override void NutrientsInputted()
        {
            //记录距离上次分解食物获取养分到本次的时间间隔
            lock (_syncObject)
            {
                Interlocked.Exchange(ref _inputInterval, _inputIntervalStopwatch.ElapsedMilliseconds);
            }
            _inputIntervalStopwatch.Restart();
        }

        protected override int GetNutrientOutputInterval()
        {
            long outputInterval = 0;
            lock (_syncObject)
            {
                outputInterval = Interlocked.Read(ref _inputInterval) / (this.NutrientsCount + 1); //+1是因为包括正在处理的这个养分
                Interlocked.Add(ref _inputInterval, -outputInterval); //每输出一次，就要减少一次统计的输入总时间_inputInterval
            }
            return (int)outputInterval;
        }

        #region 输入养分的时间间隔

        private Stopwatch _inputIntervalStopwatch;
        private long _inputInterval;

        private void InitInputInterval()
        {
            _inputIntervalStopwatch = new Stopwatch();
            _inputInterval = 0;
        }

        private void DisposeInputInterval()
        {

        }

        #endregion

        #region 计算输出所经过的时间

        private Stopwatch _outputElapsedStopwatch;

        private void InitOutputElapsed()
        {
            _outputElapsedStopwatch = new Stopwatch();
        }

        private void DisposeOutputElapsed()
        {

        }

        #endregion

        protected override void OnDispose()
        {
            DisposeInputInterval();
            DisposeOutputElapsed();
        }

    }


    /// <summary>
    /// 温饱模式，1个食物分解1个养分
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="N"></typeparam>
    public class SatietySingle<F, N> : SatietyPattern<F, N>
    {
        public SatietySingle(BreakDownHandler howBreakDown, SatietyPatternConfig config)
            : base(howBreakDown, null, config)
        {
        }
    }

    /// <summary>
    /// 温饱模式，1个食物分解多个养分
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="N"></typeparam>
    public class SatietyMultiple<F, N> : SatietyPattern<F, N>
    {
        public SatietyMultiple(BreakDownsHandler howBreakDowns, SatietyPatternConfig config)
            : base(null, howBreakDowns, config)
        {
        }
    }
}