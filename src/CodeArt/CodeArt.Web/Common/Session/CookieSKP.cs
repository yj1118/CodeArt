using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Web
{
    /// <summary>
    /// sessionKey生成器
    /// </summary>
    public sealed class CookieSKP : ISessionKeyProvider
    {
        private CookieSKP() { }

        public string LoadKey()
        {
            var key = Cookie.GetItem("__sessionId");
            if (key == null)
            {
                key = Guid.NewGuid().ToString();
                SaveKey(key);
            }
            return key;
        }

        private const int _yearMinutes = 60 * 24 * 365;

        public void SaveKey(string sessionKey)
        {
            Cookie.SetItem("__sessionId", sessionKey, _yearMinutes);//永久保留sessionId
        }

        public void RemoveKey()
        {
            Cookie.RemoveItem("__sessionId");
        }


        public bool ContainsKey()
        {
            return Cookie.GetItem("__sessionId") != null;
        }

        public static ISessionKeyProvider Instance = new CookieSKP();

    }
}
