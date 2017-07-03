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
using CodeArt.Runtime;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 目前最大的问题是sleep等待不精确
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="N"></typeparam>
    public abstract class EatPattern<F,N> : IDisposable
    {
        private EatPatternConfig _config;
        private Stopwatch _outputElapsedStopwatch;
        private ReaderWriterLockSlim _lock;

        #region 器官
        private OneByOnePipeline _stomach; //胃
        private AutoResetPipeline _enzyme; //酶
        #endregion


        #region 食物

        private Compounds<F> _foods; //食物

        public int FoodCount
        {
            get
            {
                return _foods.Count;
            }
        }

        #endregion

        #region 养分

        /// <summary>
        /// 
        /// </summary>
        /// <param name="food"></param>
        /// <param name="souccess">true:成功分解养分</param>
        /// <returns></returns>
        public delegate N BreakDownHandler(F food, out bool souccess);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="food"></param>
        /// <param name="souccess">true:成功分解养分</param>
        /// <returns></returns>
        public delegate N[] BreakDownsHandler(F food, out bool souccess);


        private Compounds<N> _nutrients; //养分
        private BreakDownsHandler _howBreakDowns; //如何将食物分解成为养分
        private BreakDownHandler _howBreakDown; //如何将食物分解成为养分

        public int NutrientsCount
        {
            get
            {
                return _nutrients.Count;
            }
        }


        #endregion

        public bool IsWorking
        {
            get
            {
                return _stomach.IsWorking || _enzyme.IsWorking;
            }
        }

        /// <summary>
        /// 输出养分
        /// </summary>
        public event Action<N> Outputted;

        private void RaiseOutputtedEvent(N nutrient)
        {
            if (this.Outputted != null)
                this.Outputted(nutrient);
        }

        private Rubbish _rubbish;

        private MediaDelayer _delapyer;

        protected EatPattern(BreakDownHandler howBreakdown, BreakDownsHandler howBreakdowns, EatPatternConfig config)
        {
            _howBreakDown = howBreakdown;
            _howBreakDowns = howBreakdowns;
            _config = config;
            _rubbish = new Rubbish();

            _stomach = new OneByOnePipeline(Digest);
            _enzyme = new AutoResetPipeline(Breakdown);
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _outputElapsedStopwatch = new Stopwatch();

            _foods = new Compounds<F>(config.MaxFoodStock, config.ReplaceMode, _rubbish); //食物
            _nutrients = new Compounds<N>(config.MaxNutrientStock, config.ReplaceMode, _rubbish); //养分
            _delapyer = new MediaDelayer();
        }

        /// <summary>
        /// 吃食物
        /// </summary>
        public void Eat(F food)
        {
            _stoped = false;

            _lock.Read(() =>
            {
                if (!CanWorking()) return;
                if (!_foods.TryAdd(food)) return;
                _enzyme.AllowOne();
            });
        }

        /// <summary>
        /// 消化食物
        /// </summary>
        private void Digest()
        {
            _lock.Read(() =>
            {
                if (!CanWorking()) return;
                bool exist = true;
                while (exist)
                {
                    exist = _nutrients.Using((nutrient) =>
                    {
                        OutputNutrient(nutrient);
                    });
                }
            });
        }

        /// <summary>
        /// 输出养分
        /// </summary>
        /// <param name="nutrient"></param>
        private void OutputNutrient(N nutrient)
        {
            if (!_outputElapsedStopwatch.IsRunning)
                _outputElapsedStopwatch.Restart();

            RaiseOutputtedEvent(nutrient);

            int nutrientOutputInterval = GetNutrientOutputInterval();
            if (nutrientOutputInterval > 0)
            {
                long sleep = 0;
                unchecked
                {
                    sleep = nutrientOutputInterval - _outputElapsedStopwatch.ElapsedMilliseconds;
                }
                if (sleep > 0)
                {
                    //var raw = _outputElapsedStopwatch.ElapsedMilliseconds;
                    //Debug.WriteLine("sleep:" + sleep);
                    _delapyer.Sleep((int)sleep);
                    //Debug.WriteLine("actualSleep:" + (_outputElapsedStopwatch.ElapsedMilliseconds - raw));
                }
            }
            _outputElapsedStopwatch.Restart();
        }

        /// <summary>
        /// 得到养分的输出间隔时间
        /// </summary>
        /// <returns></returns>
        protected abstract int GetNutrientOutputInterval();

        /// <summary>
        /// 分解食物，得到养分
        /// </summary>
        private void Breakdown()
        {
            _lock.Read(() =>
            {
                if (!CanWorking()) return;

                _foods.Using((food) =>
                {
                    bool success = false;
                    if (_howBreakDowns != null)
                    {
                        bool breakSuccess = false;
                        var nutrients = _howBreakDowns(food, out breakSuccess);
                        if (breakSuccess)
                        {
                            foreach (var nutrient in nutrients)
                            {
                                if (_nutrients.TryAdd(nutrient))
                                {
                                    success = true;
                                }
                            }
                        }
                    }
                    else if (_howBreakDown != null)
                    {
                        bool breakSuccess = false;
                        var nutrient = _howBreakDown(food, out breakSuccess);
                        if (breakSuccess)
                        {
                            if (_nutrients.TryAdd(nutrient))
                            {
                                success = true;
                            }
                        }
                    }
                    if (success)
                    {
                        NutrientsInputted();
                        _stomach.AllowOne();
                    }
                });
            });
        }

        /// <summary>
        /// 有新的养分被输入到队列中
        /// </summary>
        protected virtual void NutrientsInputted() { }


        #region 停止工作

        private bool _stoped;

        public void Stop()
        {
            if (_stoped) return;
            _stoped = true;
            _lock.Write(() =>
            {
                DisposeOverage();
            });
        }

        #endregion

        private void DisposeOverage()
        {
            _nutrients.Dispose();
            _foods.Dispose();
            _rubbish.Dispose();
        }

        private bool CanWorking()
        {
            return !(_stoped || _disposed);
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _lock.Write(() =>
            {
                DisposeOverage();
                _stomach.Dispose();
                _enzyme.Dispose();
                _delapyer.Dispose();
                OnDispose();
            });
            _lock.Dispose();
        }

        protected virtual void OnDispose() { }

    }
}
