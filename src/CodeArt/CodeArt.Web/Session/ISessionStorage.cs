using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Web
{
    public interface ISessionStorage
    {
        object Load(string sessionId, string itemId);
        void Save(string sessionId, string itemId, object value);

        void DeleteItem(string sessionId, string itemId);

        /// <summary>
        /// 删除该会话的所有数据
        /// </summary>
        /// <param name="sessionId"></param>
        void DeleteItems(string sessionId);

        /// <summary>
        /// <para>移除在存储区中停留时间超过<paramref name="minutes"/>的数据</para>
        /// <para>minutes小于或等于0代表删除所有数据</para>
        /// </summary>
        /// <param name="minutes"></param>
        void Clear(int minutes);
    }
}
