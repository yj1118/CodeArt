using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace CodeArt.Web.WebPages
{
    public class ForeverClientCache : ClientCacheBase
    {
        public override bool IsExpired(ResolveRequestCache controller)
        {
            var context = controller.Context;
            string modified =null;
            if (TryGetModified(context.Request, out modified))
            {
                SetClientStatusCode(context.Response);
                return false;
            }
            return true;
        }

        public override void SetCache(ResolveRequestCache controller)
        {
            var context = controller.Context;
            SetClientCache(context.Response, 525600);
        }

        public static readonly IClientCache Instance = new ForeverClientCache();

    }
}