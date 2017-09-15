using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 一次性信号探测器，可以检测出在指定的时间范围内是否收到信号
    /// 从第一次Receive开始检测,请注意，探测器内部采用的是Timer，属于不精确探测，会有一定的误差
    /// 与RepeatedDetector不同，该探测器检查到中断后会停止工作，就算再次Receive也不会继续工作
    /// </summary>
    public class OneOffDetector : SignalDetector
    {
        private object _syncObject = new object();

        private Action _startUpAction;

        /// <summary>
        /// 每次检查信号经过的时间间隔，单位毫秒
        /// </summary>
        /// <param name="startUpAction">当第一次收到信号时触发</param>
        /// <param name="interruptedAction">当信号中断时触发</param>
        /// <param name="intervalMilliseconds">每次检查信号经过的时间间隔，单位秒</param>
        public OneOffDetector(Action startUpAction, Action interruptedAction, int intervalMilliseconds)
            : base(interruptedAction, intervalMilliseconds)
        {
            _startUpAction = startUpAction;
        }

        public OneOffDetector(Action interruptedAction, int intervalMilliseconds)
            : this(null, interruptedAction, intervalMilliseconds)
        {
        }

      

        /// <summary>
        /// 接收信号，使用该方法可以告诉信号探测器接收到新的信号
        /// </summary>
        public void Receive()
        {
            if (!this.Start()) return;
            lock (_syncObject)
            {
                RefreshReceive();
            }
        }

        /// <summary>
        /// 开始检测
        /// </summary>
        public bool Start()
        {
            lock (_syncObject)
            {
                if (_stoped) return false;
                InitTimer();
            }
            OnStartUp();
            return true;
        }

        protected override bool HandleExpired()
        {
            this.Stop();
            return false;
        }

        private bool _stoped;

        /// <summary>
        /// 停止探测
        /// </summary>
        public void Stop()
        {
            lock (_syncObject)
            {
                _stoped = true;
                DisposeTimer();
            }
        }

        protected void OnStartUp()
        {
            if (_startUpAction == null) return;
            _startUpAction();
        }

        public override void Dispose()
        {
            this.Stop();
        }

    }
}
