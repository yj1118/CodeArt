using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 基于视图的会话，该会话在单个页面访问、回调期间有效，每次提交视图都会提交全局视图会话数据
    /// </summary>
    public class ScriptViewSession
    {
        private DTObject _data;
        private ScriptView _owner;

        internal ScriptViewSession(ScriptView owner, DTObject data)
        {
            _owner = owner;
            _data = data;
        }

        /// <summary>
        /// 设置会话数据，该方法会向客户端发射一条命令
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Set(string name, object value)
        {
            _data.SetValue(name, value);
            _owner.WriteCode(string.Format("$$view.session.set('{0}',{1});", name, JSON.GetCode(value)));
        }

        public void Set(string name, DTObject value)
        {
            _data.SetValue(name, value);
            _owner.WriteCode(string.Format("$$view.session.set('{0}',{1});", name, value.GetCode()));
        }

        /// <summary>
        /// 获取会话数据，该方法不会向客户端发射命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name, T defaultValue)
        {
            return _data.GetValue<T>(name, defaultValue);
        }

        /// <summary>
        /// 获取会话数据，该方法不会向客户端发射命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Get<T>(string name)
        {
            return _data.GetValue<T>(name);
        }

    }
}
