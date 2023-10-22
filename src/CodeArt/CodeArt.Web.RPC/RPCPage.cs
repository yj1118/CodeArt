using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.IO;
using CodeArt.AOP;
using CodeArt.Log;
using CodeArt.AppSetting;

namespace CodeArt.Web.RPC
{
    [SafeAccess]
    public sealed class RPCPage : WebText
    {
        private RPCPage() { }

        protected override string GetTextContent(string error)
        {
            var paras = this.GetQueryValues();
            var args = DTObject.Create();
            foreach (var key in paras.AllKeys)
            {
                args[key] = paras[key];
            }
            var clear = this.GetQueryValue<string>("clear", null);
            if(clear != null)
            {
                ClearCache(args);
                return string.Empty;
            }
            else
            {
                return CallWebMethod(args);
            }
        }

        private void ClearCache(DTObject args)
        {
            var procedure = ProcedureFactory.Create(this.VirtualPath);
            var cache = procedure.GetCache();
            if (cache == null) return;

            var clear = args.Dynamic.clear;
            if (clear == "all")
            {
                cache.Clear();
                return;
            }

            var key = GetCacheKey(procedure, args);
            if (string.IsNullOrEmpty(key)) return;

            cache.Remove(key);
        }

        #region 事件

        /// <summary>
        /// 请求是否只是为了效验权限
        /// </summary>
        /// <returns></returns>
        private bool IsAuthVerify()
        {
            return this.Request.QueryString["auth"] == "permission";
        }


        protected override string CallWebMethod(DTObject args)
        {
            var procedure = ProcedureFactory.Create(this.VirtualPath);

            if(this.IsAuthVerify())
            {
                return ValidateAuth(procedure, args) ? "{'auth':true}" : "{'auth':false}";
            }
            else
            {
                WriteLog(procedure, args);
                if(!ValidateAuth(procedure, args))
                {
                    throw new UserUIException("无此权限");
                }
                var content = FromCache(procedure, args);
                if (content != null) return content;

                var result = procedure.Invoke(args);
                return procedure.GetResponseCode(result);
            }
        }

        private void WriteLog(IProcedure procedure, DTObject args)
        {
            var content = procedure.GetLogContent(args);
            if (content != null && !content.IsEmpty())
            {
                Logger.Write(content);
            }
        }

        private bool ValidateAuth(IProcedure procedure, DTObject args)
        {
            if (AppSession.IgnoreAuth) return true;

            var auths = procedure.Auths;
            if (auths == null || auths.Count() == 0) return true;

            var paras = DTObject.Create();
            {
                var principalId = AppSession.PrincipalId;
                var tenantId = AppSession.TenantId;
                var dark = AppSession.Dark;

                if (!string.IsNullOrEmpty(principalId)) paras["target.principalId"] = principalId;
                if (tenantId > 0) paras["target.tenantId"] = tenantId;
                paras["target.dark"] = dark;
            }

            paras["data"] = procedure.GetAuthData(args);

            foreach (var auth in auths)
            {
                if (auth.Verify(paras))
                    return true; //验证通过
            }
            return false;
        }

        private string GetCacheKey(IProcedure procedure, DTObject args)
        {
            var cacheKey = procedure.GetCacheKey(args);
            if (string.IsNullOrEmpty(cacheKey)) return null;

            return string.Format("{0} - {1}", this.VirtualPath.ToLower(), cacheKey); //从参数得到唯一标示
        }

        private string FromCache(IProcedure procedure, DTObject args)
        {
            var cache = procedure.GetCache();
            if (cache == null) return null;

            var cacheKey = procedure.GetCacheKey(args);
            if (string.IsNullOrEmpty(cacheKey)) return null;

            var key = GetCacheKey(procedure, args);
            if (string.IsNullOrEmpty(key)) return null;

            string content = null;
            //从缓存中获取
            if (!cache.TryGet(key, out content))
            {
                lock (cache)
                {
                    if (!cache.TryGet(key, out content))
                    {
                        //加载后存入缓存
                        var result = procedure.Invoke(args);
                        content = result.GetCode(false, false);
                        cache.AddOrUpdate(key, content);
                    }
                }
            }
            return content;
        }

        #endregion

        public static readonly RPCPage Instance = new RPCPage();

    } 
}
