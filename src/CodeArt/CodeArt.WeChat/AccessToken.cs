using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web;
using CodeArt.Caching.Redis;
using CodeArt.DTO;

namespace CodeArt.WeChat
{
    public class AccessToken
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        public int ExpiresIn
        {
            get;
            private set;
        }

        private AccessToken(string value, int expiresIn)
        {
            this.Value = value;
            this.ExpiresIn = expiresIn;
        }



        private static object _syncObject = new object();
        private static AccessToken _current = new AccessToken(string.Empty, 0);

        public static AccessToken Current
        {
            get
            {
                var dto = _cache.Get("default");
                var value = dto.GetValue<string>("access_token");

                if (value != _current.Value)
                {
                    lock (_syncObject)
                    {
                        if (value != _current.Value)
                        {
                            var expiresIn = dto.GetValue<int>("expires_in");
                            _current = new AccessToken(value, expiresIn);
                        }
                    }
                }

                return _current;
            }
        }


        private static CachePortal _cache = CachePortal.UseDefault("wx-accessToken", 100, (key) =>
        {
            var appId = WeChatConfiguration.Current.MPAppId;
            var appSecret = WeChatConfiguration.Current.MPAppSecret;
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appId, appSecret);
            var reuslt = WebUtil.SendGet(url);
            var data = Encoding.UTF8.GetString(reuslt);

            var dto = DTObject.Create(data);
            var token = dto.GetValue<string>("access_token", string.Empty);

            if (string.IsNullOrEmpty(token))
            {
                throw new UserUIException("获取access_token失败");
            }

            return dto;
        });

    }
}
