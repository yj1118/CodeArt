using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Web
{
    /// <summary>
    /// sessionKey生成器
    /// </summary>
    public interface ISessionKeyProvider
    {
        string LoadKey();
        void SaveKey(string sessionKey);

        void RemoveKey();

        /// <summary>
        /// 检查提供者中是否已含有当前会话的key
        /// </summary>
        bool ContainsKey();
    }
}
