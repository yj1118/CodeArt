using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.Log;
using CodeArt.Runtime;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 不间断的行为
    /// </summary>
    public sealed class IntervalAction : IDisposable
    {
        private Task _task;
        private CancellationTokenSource _cancellation;

        private Action<IntervalAction> _action;
        private Action<Exception> _catchException;

        private int _intervalMilliseconds;

        private MediaDelayer _delapyer;

        /// <summary>
        /// 不停的执行行为，间隔时间为<paramref name="intervalMilliseconds"/>
        /// </summary>
        /// <param name="intervalMilliseconds"></param>
        /// <param name="action"></param>
        /// <param name="catchException"></param>
        public IntervalAction(int intervalMilliseconds, Action<IntervalAction> action, Action<Exception> catchException)
        {
            _intervalMilliseconds = intervalMilliseconds;
            _action = action;
            _catchException = catchException;
            _delapyer = new MediaDelayer();
            StartTask();
        }

        public IntervalAction(int intervalMilliseconds, Action<IntervalAction> action)
            : this(intervalMilliseconds, action, (e) =>
            {
                Logger.Fatal(e);
            })
        {

        }

        public IntervalAction(TimeSpan interval, Action<IntervalAction> action)
            : this((int)interval.TotalMilliseconds, action)
        {

        }

        public IntervalAction(TimeSpan interval, Action<IntervalAction> action, Action<Exception> catchException)
            : this((int)interval.TotalMilliseconds, action, catchException)
        {
        }

        private void StartTask()
        {
            _cancellation = new CancellationTokenSource();
            _task = Task.Factory.StartNew(InternalAction, _cancellation.Token);
        }

        private void InternalAction()
        {
            try
            {
                while (!_cancellation.IsCancellationRequested)
                {
                    _action(this);
                    _delapyer.Sleep(_intervalMilliseconds);
                }
            }
            catch (ThreadAbortException) { }
            catch (TaskCanceledException) { }
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
                _cancellation.Cancel();
                _delapyer.Dispose();
                //task不需要手工回收
            }
        }

    }
}
