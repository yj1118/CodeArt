using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

using CodeArt.Concurrent;
using CodeArt.TestTools;
using CodeArt.Concurrent.Pattern;
using CodeArt.Runtime;

namespace CodeArtTest.Concurrent.Eat
{
    public class SatietyTestHelper
    {
        public struct TimeRange
        {
            public int Max { get; private set; }

            public int Min { get; private set; }

            public TimeRange(int min, int max)
            {
                this.Min = min;
                this.Max = max;
            }

        }

        private readonly bool _single;
        private readonly int _maxFoodStock;
        private readonly int _maxNutrientStock;
        private readonly int _foodCount;
        private readonly int _eatFoodInterval;

        private readonly TimeRange _breakDownTime;
        private readonly TimeRange _nutrientOutputRunTime;


        private double _minNutrientOutputInterval;
        private double _maxNutrientOutputInterval;

        private MediaDelayer _delapyer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="single">是否为单个食物输出单个养分</param>
        /// <param name="foodCount">喂的食物个数</param>
        /// <param name="maxFoodStock">最大食物储备量</param>
        /// <param name="maxNutrientStock">最大养分储备量</param>
        /// <param name="breakDownTime">粉碎食物的时间</param>
        /// <param name="nutrientOutputRunTime">养分输出的执行时间</param>
        public SatietyTestHelper(bool single, int foodCount, int eatFoodInterval, int maxFoodStock,
                            int maxNutrientStock,
                            TimeRange breakDownTime,
                            TimeRange nutrientOutputRunTime)
        {
            _single = single;
            _foodCount = foodCount;
            _eatFoodInterval = eatFoodInterval;
            _maxFoodStock = maxFoodStock;
            _maxNutrientStock = maxNutrientStock;
            _breakDownTime = breakDownTime;
            _nutrientOutputRunTime = nutrientOutputRunTime;

            //实际输出时间应该在指定时间上下浮动20%
            var nutrientOutputInterval = single ? eatFoodInterval : eatFoodInterval / _nutrientsGroupCount;
            _maxNutrientOutputInterval = (double)nutrientOutputInterval * 1.2;
            _minNutrientOutputInterval = (double)nutrientOutputInterval * 0.8;
            _delapyer = new MediaDelayer();
        }

        public bool IsCorrect(long elapsed)
        {
            return elapsed <= _maxNutrientOutputInterval && elapsed >= _minNutrientOutputInterval;
        }

        public bool IsCorrect(long[] elapseds)
        {
            var average = (long)elapseds.Average();
            return this.IsCorrect(average);
        }

        EatPattern<Food, Nutrient> _eat;

        public long[] Start()
        {
            _elapseds.Clear();

            EatPattern<Food, Nutrient> eat = _single ? GetSatietySingle() : GetSatietyMultiple();
            eat.Outputted += Outputted;
            _eat = eat;

            for (var i = 0; i < _foodCount; i++)
            {
                _delapyer.Sleep(_eatFoodInterval);  //模拟吃食物的间隔时间
                eat.Eat(new Food());
            }

            while(eat.IsWorking)
            {
                Thread.Sleep(5);
            }

            return _elapseds.ToArray();
        }

        private EatPattern<Food, Nutrient> GetSatietySingle()
        {
            return new SatietySingle<Food, Nutrient>(GetNutrient, new SatietyPatternConfig(_maxFoodStock, _maxNutrientStock, CompoundReplaceMode.HappyNew));
        }

        private EatPattern<Food, Nutrient> GetSatietyMultiple()
        {
            return new SatietyMultiple<Food, Nutrient>(GetNutrients, new SatietyPatternConfig(_maxFoodStock, _maxNutrientStock, CompoundReplaceMode.HappyNew));
        }


        private Stopwatch _stopwatch = new Stopwatch();
        private int _times = 0;
        private List<long> _elapseds = new List<long>();

        private void Outputted(Nutrient nutrient)
        {
            if(_single)
            {
                if (_stopwatch.IsRunning)
                {
                    var elapsed = _stopwatch.ElapsedMilliseconds;
                    _elapseds.Add(elapsed);
                }
                _stopwatch.Restart();
            }
            else
            {
                //在1个食物输出多个养分的情况下，第一组养分的输入不算入_elapseds
                //这是因为Satiety模式下，第一次分解食物是没有统计时间的，只有第二次分解食物才能统计出第一次到第二次之间的时间
                if(_times >= _nutrientsGroupCount)
                {
                    if (_stopwatch.IsRunning)
                    {
                        var elapsed = _stopwatch.ElapsedMilliseconds;
                        _elapseds.Add(elapsed);
                    }
                    _stopwatch.Restart();
                }
            }
            _times++;

            var sleep = new Random().Next(_nutrientOutputRunTime.Min, _nutrientOutputRunTime.Max);
            Thread.Sleep(sleep);
        }

        private int _maxNutrientCount;

        public int MaxNutrientCount
        {
            get
            {
                return _maxNutrientCount;
            }
        }

        private int _digestedFoodCount = 0;

        /// <summary>
        /// 消化的食物个数
        /// </summary>
        public int DigestedFoodCount
        {
            get
            {
                return _digestedFoodCount;
            }
        }

        private Nutrient GetNutrient(Food food,out bool success)
        {
            Interlocked.Increment(ref _digestedFoodCount);

            lock (_eat)
            {
                if (_eat.NutrientsCount > _maxNutrientCount)
                    _maxNutrientCount = _eat.NutrientsCount;
            }

            var sleep = new Random().Next(_breakDownTime.Min, _breakDownTime.Max);
            Thread.Sleep(sleep);
            success = true;
            return new Nutrient();
        }

        private const int _nutrientsGroupCount = 5;

        private Nutrient[] GetNutrients(Food food, out bool success)
        {
            Interlocked.Increment(ref _digestedFoodCount);

            lock (_eat)
            {
                if (_eat.NutrientsCount > _maxNutrientCount)
                    _maxNutrientCount = _eat.NutrientsCount;
            }

            var sleep = new Random().Next(_breakDownTime.Min, _breakDownTime.Max);
            Thread.Sleep(sleep);
            List<Nutrient> nutrients = new List<Nutrient>();
            for (var i = 0; i < _nutrientsGroupCount; i++)
            {
                nutrients.Add(new Nutrient());
            }
            success = true;
            return nutrients.ToArray();
        }

        private class Food
        {

        }


        private class Nutrient
        {

        }
    }
}
