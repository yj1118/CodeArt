using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;

namespace CodeArt.Concurrent.Pattern
{
    public abstract class SignalDetector : IDisposable
    {
        /// <summary>
        /// 每次检查信号经过的时间间隔，单位毫秒
        /// </summary>
        private readonly double _intervalMilliseconds; //检测时间的间隔，默认5000毫秒

        private object _syncObject = new object();

        private Action _interruptedAction;

        /// <summary>
        /// 每次检查信号经过的时间间隔，单位毫秒
        /// </summary>
        /// <param name="interruptedAction">当信号中断时触发</param>
        /// <param name="intervalMilliseconds">每次检查信号经过的时间间隔，单位秒</param>
        public SignalDetector(Action interruptedAction, int intervalMilliseconds)
        {
            _interruptedAction = interruptedAction;
            _intervalMilliseconds = (double)intervalMilliseconds;
        }


        /// <summary>
        /// 最后一次收到信号的时间
        /// </summary>
        private DateTime _lastReceived = DateTime.Now;

        protected void RefreshReceive()
        {
            _lastReceived = DateTime.Now;
        }

        protected bool IsExpired()
        {
            return DateTime.Now.Subtract(_lastReceived).TotalMilliseconds > _intervalMilliseconds;
        }

        private Timer _timer;

        protected void InitTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(_intervalMilliseconds);
                _timer.Elapsed += OnElapsed;
                _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
                _timer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件

                //开启定时器
                _timer.Start();
            }
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            bool isInterrupted = false;
            lock (_syncObject)
            {
                if (_timer != null)
                {
                    if (IsExpired())
                    {
                        isInterrupted = true;
                        if(HandleExpired())
                        {
                            _timer.Start(); //继续探测
                        }
                    }
                    else
                    {
                        _timer.Start(); //继续探测
                    }
                }
            }
            if (isInterrupted)
            {
                //已中断
                OnInterrupted();
            }
        }


        /// <summary>
        /// 处理过期
        /// </summary>
        /// <returns>继续探测返回true</returns>
        protected abstract bool HandleExpired();

        protected void DisposeTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        protected void OnInterrupted()
        {
            if (_interruptedAction == null) return;
            _interruptedAction();
        }

        public abstract void Dispose();
    }
}
