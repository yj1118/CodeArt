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


namespace CodeArt.Web.Mobile
{
    [SafeAccess]
    public sealed class RPCPage : WebText
    {
        private RPCPage() { }

        protected override string GetTextContent(string error)
        {
            throw new NotImplementedException();
        }

        #region 事件

        protected override string CallWebMethod(DTObject args)
        {
            var procedure = ProcedureFactory.Create(this.VirtualPath);
            var content = FromCache(procedure, args);
            if (content != null) return content;

            var result = procedure.Invoke(args);
            return result.GetCode(false, false);
        }


        private string FromCache(IProcedure procedure, DTObject args)
        {
            var cache = procedure.GetCache();
            if (cache == null) return null;

            var key = string.Format("{0} - {1}",this.VirtualPath ,args.GetCode(true, false)); //从参数得到唯一标示

            {
                //从缓存中获取
                if (cache.TryGet(key, out var content))
                {
                    return content;
                }
            }

            {
                //加载后存入缓存
                var result = procedure.Invoke(args);
                var content = result.GetCode(false, false);
                cache.AddOrUpdate(key, content);
                return content;
            }
        }

        #endregion

        public static readonly RPCPage Instance = new RPCPage();

    } 
}
