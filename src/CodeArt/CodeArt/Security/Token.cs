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
    /// 令牌 token 工具类
    /// </summary>
    public static class Token
    {
        private static readonly IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        private static readonly IJsonSerializer serializer = new JsonNetSerializer();
        private static readonly IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();

        private static string Decode(string secret, string token)
        {
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
            return decoder.Decode(token, secret, verify: true);
        }

        public static DTObject Parse(string token)
        {
            return Parse(LocalSecret, token);
        }

        /// <summary>
        /// 解析token，不做验证
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static DTObject Parse(string secret, string token)
        {
            var json = Decode(secret, token);

            var dto = DTObject.Create(json);
            var code = dto.GetValue<string>("code");
            return DTObject.Create(code);
        }

        #region 创建

        /// <summary>
        /// 创建令牌
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Create(DTObject content)
        {
            return Create(LocalSecret, content);
        }

        public static string Create(string secret, DTObject content)
        {
            var payload = new Dictionary<string, object>
            {
                { "code", content.GetCode(false,false) },
                { "time", DateTime.Now.Ticks }
            };

            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            return token;
        }

        #endregion 

        #region 验证

        public static DTObject Verify(string token, float expired)
        {
            var time = DateTime.Now;
            return Verify(LocalSecret, token, expired, ref time);
        }

        public static DTObject Verify(string token, float expired, ref DateTime time)
        {
            return Verify(LocalSecret, token, expired, ref time);
        }

        public static DTObject Verify(string secret, string token, float expired)
        {
            var time = DateTime.Now;
            return Verify(secret, token, expired, ref time);
        }

        public static DTObject Verify(string secret, string token, float expired, ref DateTime time)
        {
            try
            {
                var json = Decode(secret, token);
                var dto = DTObject.Create(json);

                //expired == 0 则表示永久不过期
                time = expired == 0 ? DateTime.MaxValue : new DateTime(dto.GetValue<long>("time"));

                if (expired > 0)
                {
                    var expireTime = time.AddHours(expired);
                    //当超过expireTime，则抛出异常 
                    if (expireTime <= DateTime.Now)
                        throw new TokenExpiredException("tokenExpired");
                }

                var code = dto.GetValue<string>("code");
                return DTObject.Create(code);
            }
            catch (TokenExpiredException ex) //验证失败
            {
                //var errorCode = ex.HResult;
                //if (errorCode == -2146233088) //TokenExpiredException，令牌过期的特定错误码
                //{
                //    throw new AuthException();
                //}
                throw new AuthException("tokenExpired");
            }
            catch (SignatureVerificationException ex) //验证失败
            {
                //var errorCode = ex.HResult;
                //if (errorCode == -2146233088) //TokenExpiredException，令牌过期的特定错误码
                //{
                //    throw new AuthException();
                //}
                throw new AuthException();
            }
        }

        #endregion

        private static readonly string LocalSecret;

        static Token()
        {
            LocalSecret = Configuration.Current.SecurityConfig.Secret;
        }

    }
}
