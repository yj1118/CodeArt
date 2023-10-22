using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace CodeArt.Security
{
    /// <summary>
    /// 身份令牌 token 工具类
    /// </summary>
    public static class IdentityToken
    {
        public static string Create(DTObject content)
        {
            return Token.Create(content);
        }

        public static DTObject Verify(string token, Action<string> refreshToken)
        {
            var time = DateTime.Now;
            var content = Token.Verify(token, Expired, ref time);

            if(refreshToken!=null)
            {
                var refreshTime = time.AddHours(Refresh);

                //当超过refreshTime时间
                if (refreshTime <= DateTime.Now)
                {
                    string newToken = Create(content);
                    refreshToken(newToken);
                }
            }
            return content;
        }

        /// <summary>
        /// token过期时间（小时）
        /// token过期时间是指，当用户指定时间内没有任何操作，就得重新登录
        /// </summary>
        public static readonly float Expired;
        private static readonly float Refresh;

        static IdentityToken()
        {
            //token过期时间是指，当用户指定时间内没有任何操作，就得重新登录
            //否则在用户活跃期间，token是会自动刷新，以保持token的值在不断的变化，进而确保了安全性
            Expired = Configuration.Current.SecurityConfig.Expired;
            Refresh = Configuration.Current.SecurityConfig.Refresh;
        }

    }
}
