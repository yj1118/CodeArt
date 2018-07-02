using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CodeArt.WPF.Controls.Playstation
{
    public static class Extensions
    {
        public static void AsyncRun(this DispatcherObject obj, Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (Work.Current != null)
                    {
                        obj.Dispatcher.Invoke(()=>
                        {
                            Work.Current.Catch(ex);
                        });
                    }
                    else
                        throw ex;
                }
            });
        }


        private static Dictionary<Guid, EventProtector<Action>> _protectors = new Dictionary<Guid, EventProtector<Action>>();


        /// <summary>
        /// 不会重复的行为，短时间内双击等操作都不会重复触发行为
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void NotRepeatedAction(this DispatcherObject obj, Guid actionId, Action<Action> action)
        {
            EventProtector<Action> protector = null;
            if (!_protectors.TryGetValue(actionId, out protector))
            {
                lock(_protectors)
                {
                    if (!_protectors.TryGetValue(actionId, out protector))
                    {
                        protector = new EventProtector<Action>();
                        _protectors.Add(actionId, protector);
                    }
                }
            }
            protector.Start(action, () => { protector.End(); });
        }

    }
}
