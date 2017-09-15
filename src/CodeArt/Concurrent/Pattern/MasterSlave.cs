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
    /// <para>主从模式是指：当主要行为执行之前，需要执行所有的slave行为</para>
    /// <para>当全部slave的注册行为都被执行完毕，master才会被执行</para>
    /// <para>每次master行为执行完毕后，所有附属行为都会被清空</para>
    /// <para>场景使用例子：在同屏分享数据时，用户退出程序，需要先结束同屏分享数据的操作，再断开连接，否则会有意外发生</para>
    /// </summary>
    public sealed class MasterSlave : IDisposable
    {
        private Action _masterAction;
        private AutoResetEvent _signal = new AutoResetEvent(false);
        private List<Slave> _slaves = new List<Slave>();
        private object _syncObject = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterAction">主要行为</param>
        public MasterSlave(Action masterAction)
        {
            _masterAction = masterAction;
        }

        public void Execute()
        {
            lock(_syncObject)
            {
                if (_slaves.Count == 0)
                {
                    _signal.Set();
                }
                else
                {
                    var salves = _slaves.ToArray();
                    foreach (var slave in salves)
                    {
                        slave.Invoke();
                    }
                }

                if (_signal.WaitOne())
                {
                    _masterAction();
                }
                this.Reset();
            }
        }

        /// <summary>
        /// 新增一个slave异步行为,请action执行的逻辑完毕后，调用slave.Complete(),以便指示行为完成
        /// 因为在异步行为中，我们无法判断行为何时是真正的执行完毕，所以需要调用方手工指示完成
        /// </summary>
        /// <param name="asyncAction"></param>
        public Slave NewSlave(Action<AsyncSlave> asyncAction)
        {
            lock (_syncObject)
            {
                var slave = new AsyncSlave(this, asyncAction);
                _slaves.Add(slave);
                return slave;
            }
        }

        /// <summary>
        /// 新增一个同步的slave行为，同步行为只的是只要调用syncAction，那么行为便结束了
        /// </summary>
        /// <param name="syncAction"></param>
        /// <returns></returns>
        public Slave NewSlave(Action syncAction)
        {
            lock (_syncObject)
            {
                var slave = new SyncSlave(this, syncAction);
                _slaves.Add(slave);
                return slave;
            }
        }

        /// <summary>
        /// 取消salve行为
        /// </summary>
        /// <param name="salve"></param>
        public void CancelSlave(Slave salve)
        {
            lock (_syncObject)
            {
                _slaves.Remove(salve);
            }
        }


        private void RemoveSlave(Slave slave)
        {
            lock (_syncObject)
            {
                _slaves.Remove(slave);
                if (_slaves.Count == 0)
                {
                    _signal.Set();
                }
            }
        }


        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            lock (_syncObject)
            {
                _slaves.Clear();
                _signal.Reset();
            }
        }


        public void Dispose()
        {
            lock (_syncObject)
            {
                _signal.Dispose();
                _slaves.Clear();
            }
        }

        public abstract class Slave
        {
            internal abstract void Invoke();
        }

        public sealed class AsyncSlave : Slave
        {
            private MasterSlave _master;
            private Action<AsyncSlave> _action;

            internal AsyncSlave(MasterSlave master, Action<AsyncSlave> action)
            {
                _master = master;
                _action = action;
            }

            internal override void Invoke()
            {
                _action(this);
            }

            /// <summary>
            /// 指示行为已经完成
            /// </summary>
            public void Complete()
            {
                _master.RemoveSlave(this);
            }

        }

        public sealed class SyncSlave : Slave
        {
            private MasterSlave _master;
            private Action _action;

            internal SyncSlave(MasterSlave master, Action action)
            {
                _master = master;
                _action = action;
            }

            internal override void Invoke()
            {
                _action();
                Complete();
            }

            /// <summary>
            /// 指示行为已经完成
            /// </summary>
            private void Complete()
            {
                _master.RemoveSlave(this);
            }

        }

    }
    
}
