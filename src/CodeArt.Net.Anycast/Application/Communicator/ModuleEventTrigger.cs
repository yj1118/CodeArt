using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent.Pattern;

namespace CodeArt.Net.Anycast
{
    internal sealed class ModuleEventTrigger
    {

        private List<ModuleChangedEventHandler> _handles = new List<ModuleChangedEventHandler>();

        private List<(RtpCommunicator communicator, RtpModule module,bool installed)> _items = new List<(RtpCommunicator communicator, RtpModule module, bool installed)>();

        private object _syncObject = new object();

        /// <summary>
        /// 挂载事件，事件一旦被挂载，会自动触发历史事件，这样就不会出现因为之前安装/卸载模块在新的界面中没有被捕获的现象了
        /// </summary>
        /// <param name="handle"></param>
        public void Hook(ModuleChangedEventHandler handle)
        {
            lock (_syncObject)
            {
                _handles.Add(handle); //将处理器收集起来
                                      //根据已安装的模块，执行处理器的方法
                foreach (var item in _items)
                {
                    ActionPipeline.Default.Queue(() =>
                    {
                        handle(this, new ModuleChangedEventArgs(item.communicator, item.module, item.installed));
                    });
                }
            }
        }


        public void Unhook(ModuleChangedEventHandler handle)
        {
            lock (_syncObject)
            {
                _handles.Remove(handle);
            }
        }

        /// <summary>
        /// 触发模块被安装的事件
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="module"></param>
        public void Raise(RtpCommunicator communicator, RtpModule module, bool installed)
        {
            lock (_syncObject)
            {
                _Remove(communicator, module); //移除之前的状态
                _items.Add((communicator, module, installed));  //将被安装的模块收集起来
                                                                //执行已经注册的安装处理器的方法
                var args = new ModuleChangedEventArgs(communicator, module, installed);
                foreach (var handle in _handles)
                {
                    ActionPipeline.Default.Queue(() =>
                    {
                        handle(this, args);
                    });
                }

                if(!installed)
                {
                    _Remove(communicator, module); //对于卸载事件，执行完毕后直接删除，不必收集，因为卸载事件的收集累加执行没有意义
                }
            }
        }

        private void _Remove(RtpCommunicator communicator, RtpModule module)
        {
            var item = _items.FirstOrDefault((t) =>
            {
                return t.communicator == communicator && t.module == module;
            });

            if (item.module != null)
            {
                _items.Remove(item);
            }
        }

        public RtpModule Get(Func<RtpModule, bool> find)
        {
            lock (_syncObject)
            {
                var item = _items.FirstOrDefault((t) =>
                {
                    return find(t.module);
                });

                if (item.module == null) return null;
                return item.module;

            }
        }

        public bool IsInstall(Func<RtpModule, bool> find)
        {
            lock (_syncObject)
            {
                var item = _items.FirstOrDefault((t) =>
                {
                    return find(t.module);
                });

                if (item.module == null) return false;
                return item.installed;
            }
        }

    }
}
