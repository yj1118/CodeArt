using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.Log;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 基于信号的行为
    /// 理论上，每触发一次信号，就执行一次行为，但是在行为执行的过程中，多次触发信号不会阻塞
    /// 行为执行的过程中，触发信号不会导致线程等待
    /// </summary>
    public abstract class AutoResetPipelineBase : IPipelineAction, IDisposable
    {
        private AutoResetEvent _executeSignal;
        private AutoResetEvent _allowSignal;

        private long _workTimes;
        public bool IsWorking
        {
            get
            {
                return Interlocked.Read(ref _workTimes) > 0;
            }
        }

        private Action _action;
        private Action<Exception> _catchException;
        private long _maxConcurrent; //最大并发数

        public AutoResetPipelineBase(Action action, Action<Exception> catchException, long maxConcurrent)
        {
            _action = action;
            _catchException = catchException;
            _maxConcurrent = maxConcurrent;
            Start();
        }

        public AutoResetPipelineBase(Action action, Action<Exception> catchException)
            : this(action, catchException, 0)
        {
        }

        public AutoResetPipelineBase(Action action)
            : this(action, (ex) =>
            {
                Logger.Fatal(ex);
            },0)
        {
        }

        public AutoResetPipelineBase(Action action, long maxConcurrent)
            : this(action, (ex) =>
            {
                Logger.Fatal(ex);
            }, maxConcurrent)
        {
        }

        private void Start()
        {
            _executeSignal = new AutoResetEvent(false);
            _allowSignal = new AutoResetEvent(true);
            _workTimes = 0;
            InitHost(BackgroundAction);
        }

        protected abstract void InitHost(Action backgroundAction);
        protected abstract void DisposeHost();

        /// <summary>
        /// 允许执行一次
        /// </summary>
        public void AllowOne()
        {
            try
            {
                if (_maxConcurrent > 0 && (Interlocked.Read(ref _workTimes) >= _maxConcurrent))
                {
                    if (_allowSignal.WaitOne())
                    {
                        Interlocked.Increment(ref _workTimes);
                        _executeSignal.Set();
                    }
                }
                else
                {
                    Interlocked.Increment(ref _workTimes);
                    _executeSignal.Set();
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
        }

        private void BackgroundAction()
        {
            try
            {
                while (!_dispose)
                {
                    if (_executeSignal.WaitOne())
                    {
                        if (_dispose) return;
                        while (Interlocked.Read(ref _workTimes) > 0)
                        {
                            try
                            {
                                _action();
                            }
                            catch(Exception e)
                            {
                                //用户行为造成的异常不影响整个通道的使用
                                _catchException(e);
                            }
                            Interlocked.Decrement(ref _workTimes);
                            //每执行一次，就可以允许一次
                            if (_maxConcurrent > 0)
                            {
                                _allowSignal.Set();
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (TaskCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                _catchException(e);
            }
        }

        private bool _dispose = false;

        public void Dispose()
        {
            if (!_dispose)
            {
                _dispose = true;
                _executeSignal.Set();
                _executeSignal.Dispose();
                _allowSignal.Set();
                _allowSignal.Dispose();
                DisposeHost();
            }
        }
    }
}
