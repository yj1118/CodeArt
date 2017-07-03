using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 锁定操作，在锁定时间内，不能重复执行操作
    /// <para>使用场景举例：开会时，与会人向主持人申请使用投影仪，此时不能在10秒内重复申请，当主持人超过60秒没有应答时，也需要给出超时提醒</para>
    /// </summary>
    public sealed class LockAction
    {
        private int _maxLockingTime; //锁定时间
        private Timer _timer;
        private object _syncObject = new object();
        private LockAction.Mode _mode;
        private Status _status;
        private DateTime _executeTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="maxLockingTime">最大锁定时间，单位秒</param>
        public LockAction(LockAction.Mode mode, int maxLockingTime)
        {
            _mode = mode;
            _maxLockingTime = maxLockingTime;
            _status = Status.Idle;
            InitTimer();
            if (mode == Mode.Queue) _actionQueue = new Queue<Action>();
        }

        private void InitTimer()
        {
            _timer = new Timer(1000); //每隔1秒执行一次
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if(e.SignalTime.Subtract(_executeTime).TotalSeconds > _maxLockingTime)
            {
                //锁定超时
                RaiseTimeout();
                return;
            }
            _timer.Start();
        }

        /// <summary>
        /// 开始为行为开启锁保护
        /// </summary>
        /// <param name="action"></param>
        public bool Begin(Action action)
        {
            if (_mode == Mode.NoRepeat)
                return BeginWithNoRepeat(action);
            else
                return BeginWithQueue(action);
        }

        /// <summary>
        /// 指示锁定结束
        /// </summary>
        public bool End()
        {
            if (_mode == Mode.NoRepeat)
                return this.EndWithNoRepeat();
            else
                return this.EndWithQueue();
        }

        #region 标记开始

        private void MarkBegin(Action action)
        {
            OnPreBegin();
            action();
            _status = Status.Working;
            _executeTime = DateTime.Now;
            //开启定时器
            _timer.Start();
        }

        public event Action PreBegin;

        private void OnPreBegin()
        {
            if (this.PreBegin != null)
                this.PreBegin();
        }

        #endregion

        #region 标记结束

        private void MarkEnd()
        {
            _status = Status.Idle;
            OnEndLock();
        }

        public event Action EndLocked;

        private void OnEndLock()
        {
            if (this.EndLocked != null)
                this.EndLocked();
        }

        #endregion

        #region 超时处理

        private void RaiseTimeout()
        {
            if (_mode == Mode.NoRepeat)
                this.TimeoutWithNoRepeat();
            else
                this.TimeoutWithQueue();
        }


        private void MarkTimeout()
        {
            _status = Status.Idle;
            OnTimeout();
        }

        /// <summary>
        /// 锁定超时,超时时间触发后，同样会结束锁，因此会触发EndLocked事件
        /// </summary>
        public event Action Timedout;

        private void OnTimeout()
        {
            if (this.Timedout != null)
                this.Timedout();
        }

        #endregion

        #region NoRepeat模式

        /// <summary>
        /// 不可重复模式，如果操作被锁定，那么新的操作将被取消
        /// </summary>
        /// <param name="action"></param>
        private bool BeginWithNoRepeat(Action action)
        {
            lock (_syncObject)
            {
                if (_status == Status.Working)
                {
                    OnRefused();
                    return false;  //锁正在工作，因此不执行新的行为
                }
                MarkBegin(action);
                return true;
            }
        }

        /// <summary>
        /// 结束锁定
        /// </summary>
        private bool EndWithNoRepeat()
        {
            lock (_syncObject)
            {
                if (_status == Status.Working)
                {
                    MarkEnd();
                    return true;
                }
                return false;
            }
        }

        private void TimeoutWithNoRepeat()
        {
            lock (_syncObject)
            {
                if (_status == Status.Working)
                {
                    OnTimeout();
                    this.End();
                }
            }
        }

        /// <summary>
        /// 行为被拒绝，这是由于在锁定期间引入了新的行为导致的，该事件仅在NoRepeat模式下有效
        /// </summary>
        public event Action Refused;

        private void OnRefused()
        {
            if (this.Refused != null)
                this.Refused();
        }


        #endregion

        #region 队列模式

        private Queue<Action> _actionQueue;

        /// <summary>
        /// 不可重复模式，如果操作被锁定，那么新的操作将放入队列中依次执行
        /// </summary>
        /// <param name="action"></param>
        private bool BeginWithQueue(Action action)
        {
            lock (_syncObject)
            {
                //如果状态是闲置，但是队列数大于0，说明正在调度新的行为，因此将action加入到队列中，而不是开始执行
                if (_status == Status.Working || _actionQueue.Count > 0)
                {
                    _actionQueue.Enqueue(action);
                    OnWaiting();
                    return false;
                }
                MarkBegin(action);
                return true;
            }
        }

        private bool EndWithQueue()
        {
            lock (_syncObject)
            {
                if (_status == Status.Working)
                {
                    MarkEnd();
                    BeginFromQueue();
                    return true;
                }
                return false;
            }
        }

        private void TimeoutWithQueue()
        {
            lock (_syncObject)
            {
                if (_status == Status.Working)
                {
                    MarkTimeout();
                    BeginFromQueue();
                }
            }
        }

        private void BeginFromQueue()
        {
            if (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();
                MarkBegin(action);
            }
        }

        /// <summary>
        /// 行为正在等待执行，这是由于在队列模式下，新的行为在锁定期间会被加入到队列中等待执行
        /// </summary>
        public event Action Waiting;

        private void OnWaiting()
        {
            if (this.Waiting != null)
                this.Waiting();
        }

        #endregion

        public enum Mode
        {
            /// <summary>
            /// 不可重复模式
            /// </summary>
            NoRepeat,
            /// <summary>
            /// 队列模式
            /// </summary>
            Queue
        }

        public enum Status
        {
            /// <summary>
            /// 锁正在工作
            /// </summary>
            Working,
            /// <summary>
            /// 锁已空闲
            /// </summary>
            Idle
        }
    }
    
}
