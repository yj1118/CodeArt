using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent.Pattern;

namespace CodeArt.Event
{
    /// <summary>
    /// 有效事件触发器，对于有效的事件会保留到历史中，一旦有新的挂载，会再次执行
    /// </summary>
    public sealed class ValidEventTrigger
    {
        private List<ValidEventHandler> _handles = new List<ValidEventHandler>();

        private List<ValidEventArgs> _args = new List<ValidEventArgs>();

        private object _syncObject = new object();

        /// <summary>
        /// 挂载事件，事件一旦被挂载，会自动触发历史事件，这样就不会因为之前成员的上下线在新的界面中没有被捕获
        /// </summary>
        /// <param name="handle"></param>
        public void Hook(ValidEventHandler handle)
        {
            lock (_syncObject)
            {
                _handles.Add(handle); //将处理器收集起来
                                      //根据已触发的事件参数，执行处理器的方法
                foreach (var arg in _args)
                {
                    ActionPipeline.Default.Queue(() =>
                    {
                        handle(this, arg);
                    });
                }
            }
        }

        /// <summary>
        /// 解绑事件
        /// </summary>
        /// <param name="handle"></param>
        public void Unhook(ValidEventHandler handle)
        {
            lock (_syncObject)
            {
                _handles.Remove(handle);
            }
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="module"></param>
        public void Raise(ValidEventArgs arg)
        {
            lock (_syncObject)
            {
                OnReceivedArgument(arg); //收到新参数

                //执行已经注册的处理器的方法
                foreach (var handle in _handles)
                {
                    ActionPipeline.Default.Queue(() =>
                    {
                        handle(this, arg);
                    });
                }

                if (arg.IsHistoric)
                {
                    //具有历史性，保留参数
                    _args.Add(arg);
                }
            }
        }

        private void OnReceivedArgument(ValidEventArgs arg)
        {
            var temp = _args.ToArray();
            foreach (var t in temp)
            {
                t.Apply(_args, arg);
            }
        }


        /// <summary>
        /// 不在触发事件
        /// </summary>
        /// <param name="needUnRaise">赛选需要不再触发事件的参数，返回true表示不在触发，返回false表示还需要触发</param>
        public void UnRaise(Func<ValidEventArgs, bool> needUnRaise)
        {
            lock (_syncObject)
            {
                for (var i = 0; i < _args.Count; i++)
                {
                    var arg = _args[i];
                    if (needUnRaise(arg))
                    {
                        _args.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
 }
