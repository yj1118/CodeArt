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

namespace CodeArtTest.Concurrent.Eat
{
    [TestClass]
    public abstract class SatietyTest
    {
        protected abstract bool IsSingle
        {
            get;
        }

        /// <summary>
        /// 理想状态
        /// 无食物上限、无养分上限
        /// 无粉碎食物的时间
        /// </summary>
        [TestMethod]
        public void IdealState()
        {
            var breakDownTime = new SatietyTestHelper.TimeRange(0, 0);
            var nutrientOutputRunTime = new SatietyTestHelper.TimeRange(0, 0);

            SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 60, 0, 0, breakDownTime, nutrientOutputRunTime);
            var elapseds = helper.Start();
            Assert.IsTrue(helper.IsCorrect(elapseds));
        }

        /// <summary>
        /// 游刃有余1
        /// 无食物上限、无养分上限
        /// 粉碎食物的时间低于输出时间间隔
        /// </summary>
        [TestMethod]
        public void MoreThanEase1()
        {
            var breakDownTime = new SatietyTestHelper.TimeRange(5, 10);
            var nutrientOutputRunTime = new SatietyTestHelper.TimeRange(0, 0);

            SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 60, 0, 0, breakDownTime, nutrientOutputRunTime);
            var elapseds = helper.Start();
            Assert.IsTrue(helper.IsCorrect(elapseds));
        }

        /// <summary>
        /// 游刃有余2
        /// 无食物上限、无养分上限
        /// 无粉碎食物的时间
        /// 输出运行时间低于输出时间间隔
        /// </summary>
        [TestMethod]
        public void MoreThanEase2()
        {
            var breakDownTime = new SatietyTestHelper.TimeRange(0, 0);
            var nutrientOutputRunTime = new SatietyTestHelper.TimeRange(5, 10);

            //吃食物的时间间隔为100毫秒
            //在1对1模式下，每个养分输出时间为100毫秒
            //在1对多模式下，每个养分输出时间为100/n（SatietyTestHelper中n为5，也就是20毫秒）
            SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 100, 0, 0, breakDownTime, nutrientOutputRunTime);
            var elapseds = helper.Start();
            Assert.IsTrue(helper.IsCorrect(elapseds));
        }


        /// <summary>
        /// 游刃有余3
        /// 无食物上限、无养分上限
        /// 粉碎食物的时间低于输出时间间隔
        /// 输出运行时间低于输出时间间隔
        /// </summary>
        [TestMethod]
        public void MoreThanEase3()
        {
            var breakDownTime = new SatietyTestHelper.TimeRange(0, 10);
            var nutrientOutputRunTime = new SatietyTestHelper.TimeRange(5, 10);

            //吃食物的时间间隔为100毫秒
            //在1对1模式下，每个养分输出时间为100毫秒
            //在1对多模式下，每个养分输出时间为100/n（SatietyTestHelper中n为5，也就是20毫秒）
            SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 100, 0, 0, breakDownTime, nutrientOutputRunTime);
            var elapseds = helper.Start();
            Assert.IsTrue(helper.IsCorrect(elapseds));
        }


        /// <summary>
        /// 高负荷1
        /// 无食物上限、无养分上限
        /// 粉碎食物的时间高于输出时间间隔
        /// 无输出时间间隔
        /// </summary>
        [TestMethod]
        public void HighLoad1()
        {
            var nutrientOutputRunTime = new SatietyTestHelper.TimeRange(0, 0);


            if(this.IsSingle)
            {
                var breakDownTime = new SatietyTestHelper.TimeRange(75, 85);
                var range = new NumericalRangeInt(80, 0.2f);

                SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 50, 0, 0, breakDownTime, nutrientOutputRunTime);
                var elapseds = helper.Start();
                Assert.IsTrue(range.Contains((int)elapseds.Average()));
            }
            else
            {
                {
                    var breakDownTime = new SatietyTestHelper.TimeRange(30, 30);

                    //吃食物的时间间隔为100毫秒
                    //在1对多模式下，每个养分输出时间为100/n（SatietyTestHelper中n为5，也就是20毫秒）
                    //我们设置粉碎时间为30毫秒，但是每粉碎1个食物，可以获得5个养分，这5个养分的输出总时间是100毫秒
                    //因此，输出的总时间是高于粉碎时间的，那么平均输出时间依然是20毫秒
                    var range = new NumericalRangeInt(20, 0.2f);
                    SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 100, 0, 0, breakDownTime, nutrientOutputRunTime);
                    var elapseds = helper.Start();
                    var average = (int)elapseds.Average();
                    Assert.IsTrue(range.Contains(average));
                }

                {
                    var breakDownTime = new SatietyTestHelper.TimeRange(200, 200);

                    //吃食物的时间间隔为100毫秒
                    //在1对多模式下，每个养分输出时间应该为100/n（SatietyTestHelper中n为5，也就是20毫秒）
                    //但是我们设置粉碎时间为200毫秒，大于吃食物的时间
                    //那么获取养分的周期应该是200毫秒，每粉碎1个食物，可以获得5个养分，这5个养分的输出间隔应该是40毫秒
                    var range = new NumericalRangeInt(40, 0.2f);
                    SatietyTestHelper helper = new SatietyTestHelper(this.IsSingle, 5, 100, 0, 0, breakDownTime, nutrientOutputRunTime);
                    var elapseds = helper.Start();
                    var average = (int)elapseds.Average();
                    Assert.IsTrue(range.Contains(average));
                }

            }


        }

        ///// <summary>
        ///// 高负荷2
        ///// 无食物上限、无养分上限
        ///// 无粉碎食物的时间
        ///// 输出运行时间高于输出时间间隔
        ///// </summary>
        //[TestMethod]
        //public void HighLoad2()
        //{
        //    var breakDownTime = new HungryTestHelper.TimeRange(0, 0);
        //    var nutrientOutputRunTime = new HungryTestHelper.TimeRange(25, 30);

        //    HungryTestHelper helper = new HungryTestHelper(this.IsSingle, 10, 0, 0, 0, 20, breakDownTime, nutrientOutputRunTime);
        //    var elapseds = helper.Start();
        //    Assert.IsTrue(IsCorrect(elapseds, 25, 30));
        //}

        ///// <summary>
        ///// 高负荷3
        ///// 无食物上限、无养分上限
        ///// 粉碎食物的时间高于输出时间间隔
        ///// 输出运行时间高于输出时间间隔
        ///// </summary>
        //[TestMethod]
        //public void HighLoad3()
        //{
        //    var breakDownTime = new HungryTestHelper.TimeRange(30, 40);
        //    var nutrientOutputRunTime = new HungryTestHelper.TimeRange(25, 30);

        //    HungryTestHelper helper = new HungryTestHelper(this.IsSingle, 10, 0, 0, 0, 20, breakDownTime, nutrientOutputRunTime);
        //    var elapseds = helper.Start();
        //    if (this.IsSingle)
        //    {
        //        Assert.IsTrue(IsCorrect(elapseds, 30, 40));
        //    }
        //    else
        //    {
        //        Assert.IsTrue(IsCorrect(elapseds, 20, 35));
        //    }
        //}


        ///// <summary>
        ///// 限制食物数量
        ///// 养分储备数量不限制
        ///// </summary>
        //[TestMethod]
        //public void LimitFoods()
        //{
        //    var breakDownTime = new HungryTestHelper.TimeRange(0, 0);
        //    var nutrientOutputRunTime = new HungryTestHelper.TimeRange(0, 0);

        //    HungryTestHelper helper = new HungryTestHelper(this.IsSingle, 10, 2, 5, 0, 20, breakDownTime, nutrientOutputRunTime);
        //    helper.Start();

        //    //由于设置了5的储备上限，那么必然有超过5的时候，food被抛弃，所以food的数量应该大于等于5并且小于10
        //    Assert.IsTrue(helper.DigestedFoodCount < 10 && helper.DigestedFoodCount >= 5);
        //}

        ///// <summary>
        ///// 不限制食物数量
        ///// 限制养分储备数量
        ///// </summary>
        //[TestMethod]
        //public void LimitNutrients()
        //{
        //    var breakDownTime = new HungryTestHelper.TimeRange(0, 0);
        //    var nutrientOutputRunTime = new HungryTestHelper.TimeRange(0, 0);

        //    HungryTestHelper helper = new HungryTestHelper(this.IsSingle, 10, 2, 0, 5, 20, breakDownTime, nutrientOutputRunTime);
        //    helper.Start();

        //    Assert.IsTrue(helper.MaxNutrientCount <= 5);
        //}


        public bool IsCorrect(long elapsed, int min, int max)
        {
            return elapsed <= max && elapsed >= min;
        }

        public bool IsCorrect(long[] elapseds, int min, int max)
        {
            var average = (long)elapseds.Average();
            return this.IsCorrect(average, min, max);
        }


    }
}
