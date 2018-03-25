using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    internal class EventCallback
    {
        private Action _action;

        public bool HasAction
        {
            get
            {
                return _action != null;
            }
        }

        /// <summary>
        /// 挂载事件方法
        /// </summary>
        /// <param name="action"></param>
        public void Mount(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            if (_action != null)
                _action();
        }
    }
}
