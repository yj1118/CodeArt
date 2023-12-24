using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web;
using CodeArt.Caching.Redis;
using CodeArt.DTO;
using CodeArt.Util;
using System.Security.Cryptography;
using CodeArt.Concurrent;

namespace CodeArt.WeChat
{
    public class JsApiTicket
    {
        /// <summary>
        /// 获取到的签名
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 签名有效时间，单位：秒
        /// </summary>
        public int ExpiresIn
        {
            get;
            private set;
        }

        private JsApiTicket(string value, int expiresIn)
        {
            this.Value = value;
            this.ExpiresIn = expiresIn;
        }


        #region 获得签名

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">需要使用微信jsapi的网页的地址，不包含#及其后面部分</param>
        /// <returns></returns>
        public static DTObject GetSignature(string url)
        {
            var nonceStr = GetRandomString(16);
            var timeStamp = GetTimeStamp();
            var ticket = JsApiTicket.Current.Value;

            var string1 = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", ticket, nonceStr, timeStamp, url);

            var data = DTObject.Create();
            data["ticket"] = ticket;
            data["appId"] = WeChatConfiguration.Current.MPAppId;
            data["nonceStr"] = nonceStr;
            data["timestamp"] = timeStamp;
            data["string1"] = string1;
            data["signature"] = Sha1(string1);
            return data;
        }


        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <param name="CodeCount"></param>
        /// <returns></returns>
        private static string GetRandomString(int CodeCount)
        {
            return Guid.NewGuid().ToString("N") + "1"; //防止dto识别为guid,自动转义
            //string allChar = "1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,i,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            //string[] allCharArray = allChar.Split(',');
            //using (var pool = StringPool.Borrow())
            //{
            //    var code = pool.Item;

            //    int temp = -1;
            //    Random rand = new Random();
            //    for (int i = 0; i < CodeCount; i++)
            //    {
            //        if (temp != -1)
            //        {
            //            rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
            //        }
            //        int t = rand.Next(allCharArray.Length - 1);
            //        while (temp == t)
            //        {
            //            t = rand.Next(allCharArray.Length - 1);
            //        }
            //        temp = t;
            //        code.Append(allCharArray[t]);
            //    }

            //    return code.ToString();
            //}
        }


        //private static String create_nonce_str()
        //{
        //    return UUID.randomUUID().toString();
        //}

        //private static String create_timestamp()
        //{
        //    return Long.toString(System.currentTimeMillis() / 1000);
        //}

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private static string GetTimeStamp()
        {
            return DateTime.Now.Timestamp().ToString();
        }

        private static string Sha1(string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA1.Create().ComputeHash(buffer);

            using (var pool = StringPool.Borrow())
            {
                var sb = pool.Item;
                foreach (var t in data)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        #endregion



        private static object _syncObject = new object();
        private static JsApiTicket _current = new JsApiTicket(string.Empty, 0);

        internal static JsApiTicket Current
        {
            get
            {
                var dto = _cache.Get("default");
                var value = dto.GetValue<string>("ticket");

                if (value != _current.Value)
                {
                    lock (_syncObject)
                    {
                        if (value != _current.Value)
                        {
                            var expiresIn = dto.GetValue<int>("expires_in");
                            _current = new JsApiTicket(value, expiresIn);
                        }
                    }
                }

                return _current;
            }
        }


        private static CachePortal _cache = CachePortal.UseDefault("wx-jsApiTicket", 100, (key) =>
        {
            var token = AccessToken.Current.Value;

            var url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", token);
            var reuslt = WebUtil.SendGet(url);
            var data = Encoding.UTF8.GetString(reuslt);
            return DTObject.Create(data);
        });
    }
}
