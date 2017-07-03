using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 队列行为，只有第一个进入队列的行为才能执行，
    /// 其后加入到的行为只有等待被执行的行为同意，才能继续执行
    /// 当队列被终止后，继续加入的行为会被直接取消执行
    /// </summary>
    public sealed class QueueAction:IDisposable
    {
        
        private Queue<Tuple<Action<QueueAction, object>, object>> _queue = new Queue<Tuple<Action<QueueAction, object>, object>>();
        private AutoResetEvent _signal = new AutoResetEvent(true); //设置为true，第一次执行WaitOne的时候，不会被锁住
        private bool _isBreak = false; //队列是否中断了
        private object _syncObject = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialState">若要将初始状态设置为可以执行，则为 true；若要将初始状态设置为中断，则为 false。</param>
        public QueueAction(bool initialState)
        {
            if (!initialState)
                this.Break();
        }

        /// <summary>
        /// 排队等待执行，只有队列开始处的第一项可以执行，其余的行为需要等待
        /// </summary>
        /// <param name="action">请务必在该行为中调用 queue.Break() 或者 queue.Contine() 以便终止或继续执行队列行为</param>
        /// <param name="args">行为执行时需要的参数</param>
        public void LineUp(Action<QueueAction, object> action, object args)
        {
            Append(action, args);

            if (_signal.WaitOne())
            {
                Execute();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">请务必在该行为中调用 queue.Break() 或者 queue.Contine() 以便终止或继续执行队列行为</param>
        public void LineUp(Action<QueueAction> action)
        {
            LineUp((queue, args) =>
            {
                action(this);
            }, null);
        }

        /// <summary>
        /// 执行队列中第一项行为，该方法是线程安全的
        /// </summary>
        private void Execute()
        {
            var tuple = GetNext();
            if (tuple == null) throw new InvalidOperationException("QueueAction.Execute错误，队列为空，不能执行Execute方法");
            var action = tuple.Item1;
            var args = tuple.Item2;

            lock (_syncObject)
            {
                if (_isBreak)
                {
                    //队列已中断，不接受新的请求
                    RaiseActionCanceled(args);
                    _signal.Set(); //即使队列中断了，也要set一次，保证下次进来排队的第一项行为可以立即执行
                }
                else
                {
                    //此处由用户代码执行 Break或者Continue
                    action(this, args);
                }
            }
        }

        #region 操作队列

        private void Append(Action<QueueAction, object> action, object args)
        {
            lock (_queue)
            {
                var tuple = Tuple.Create(action, args);
                _queue.Enqueue(tuple);
            }
        }

        private Tuple<Action<QueueAction, object>, object> GetNext()
        {
            lock (_queue)
            {
                if (_queue.Count > 0)
                {
                    return _queue.Dequeue();
                }
            }
            return null;
        }

        private void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        #endregion

        /// <summary>
        /// 指示队列不在接受行为的执行，已加入队列的行为都会被取消执行
        /// 即将加入的队列行为会被直接取消，不进入队列
        /// </summary>
        public void Break()
        {
            lock (_syncObject)
            {
                _isBreak = true;
                _signal.Set();
            }
        }


        /// <summary>
        /// 继续执行下一个行为
        /// </summary>
        public void Continue()
        {
            lock (_syncObject)
            {
                _isBreak = false;
                _signal.Set(); //就算被重复set了多次，也只会有一次的效果
            }
        }


        private void RaiseActionCanceled(object args)
        {
            if (this.ActionCanceled != null)
                this.ActionCanceled(args);
        }

        public event Action<object> ActionCanceled;



        public void Dispose()
        {
            _signal.Dispose();
        }

    }
    
}
