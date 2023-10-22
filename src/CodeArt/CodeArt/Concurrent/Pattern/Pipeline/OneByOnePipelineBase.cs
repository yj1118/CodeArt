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
    /// 一个接一个的执行行为，每允许一次就执行一次行为
    /// 允许和执行是成对执行的
    /// </summary>
    public abstract class OneByOnePipelineBase : IPipelineAction, IDisposable
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

        public OneByOnePipelineBase(Action action, Action<Exception> catchException)
        {
            _action = action;
            _catchException = catchException;
            Start();
        }

        public OneByOnePipelineBase(Action action)
            : this(action, (ex) =>
            {
                Logger.Fatal(ex);
            })
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
                if(_allowSignal.WaitOne())
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

                        while(Interlocked.Read(ref _workTimes) > 0)
                        {
                            try
                            {
                                _action();
                            }
                            catch (Exception e)
                            {
                                //用户行为造成的异常不影响整个通道的使用
                                _catchException(e);
                            }
                            Interlocked.Decrement(ref _workTimes);
                        }
                        _allowSignal.Set();
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
