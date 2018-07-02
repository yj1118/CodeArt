using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WebPageSetting
    {
        private List<ResidentItem> _items = null;

        public void AddItem(string configName, NameValueCollection args)
        {
            if (_items == null) _items = new List<ResidentItem>();
            _items.Add(new ResidentItem(configName, args));
        }

        public WebPageSetting()
        {
        }

        public string GetConfigValue(string itemName, string argName)
        {
            if (_items == null) return null;
            ResidentItem item = _items.FirstOrDefault(o => o.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            return item != null ? item.GetValue(argName) : null;
        }


        /// <summary>
        /// 
        /// </summary>
        private sealed class ResidentItem
        {
            private string _name;
            /// <summary>
            /// 参数名称
            /// </summary>
            public string Name
            {
                get { return _name; }
            }

            private NameValueCollection _args = null;

            /// <summary>
            /// 增加参数
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public void Add(string name, string value)
            {
                _args.Add(name, value);
            }

            /// <summary>
            /// 获取参数值
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public string GetValue(string argName)
            {
                return _args[argName];
            }

            public ResidentItem(string name, NameValueCollection args)
            {
                _name = name;
                _args = args;
            }
        }


    }
}
