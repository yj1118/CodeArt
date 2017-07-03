using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 可以重复使用的信号探测器，可以检测出在指定的时间范围内是否收到信号
    /// 从第一次Receive开始检测,请注意，探测器内部采用的是Timer，属于不精确探测，会有一定的误差
    /// </summary>
    public class RepeatedDetector : SignalDetector
    {
        private object _syncObject = new object();

        private bool _stopOnInterrupted;

        /// <summary>
        /// 每次检查信号经过的时间间隔，单位毫秒
        /// </summary>
        /// <param name="interruptedAction">当信号中断时触发</param>
        /// <param name="intervalMilliseconds">每次检查信号经过的时间间隔，单位秒</param>
        /// <param name="stopOnInterrupted">当信号中断后，是否停止探测,true停止，false不停止，注意，信号中断后，如果再次Receive收到信号，那么会进行新的一轮信号探测</param>
        public RepeatedDetector(Action interruptedAction, int intervalMilliseconds, bool stopOnInterrupted)
            : base(interruptedAction, intervalMilliseconds)
        {
            _stopOnInterrupted = stopOnInterrupted;
        }

        /// <summary>
        /// 如果发生一次中断，那么就不继续探测，注意，信号中断后，如果再次Receive收到信号，那么会进行新的一轮信号探测
        /// </summary>
        /// <param name="interruptedAction"></param>
        /// <param name="intervalMilliseconds"></param>
        public RepeatedDetector(Action interruptedAction, int intervalMilliseconds)
            : this(interruptedAction, intervalMilliseconds, true)
        {
        }
     
        /// <summary>
        /// 接收信号，使用该方法可以告诉信号探测器接收到新的信号
        /// </summary>
        public void Receive()
        {
            this.Start();
            lock(_syncObject)
            {
                RefreshReceive();
            }
        }

        /// <summary>
        /// 开始检测
        /// </summary>
        public void Start()
        {
            lock (_syncObject)
            {
                InitTimer();
            }
        }

        protected override bool HandleExpired()
        {
            if (_stopOnInterrupted)
            {
                this.Stop(); //停止探测
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止探测
        /// </summary>
        public void Stop()
        {
            lock (_syncObject)
            {
                DisposeTimer();
            }
        }

        public override void Dispose()
        {
            this.Stop();
        }
    }
}
