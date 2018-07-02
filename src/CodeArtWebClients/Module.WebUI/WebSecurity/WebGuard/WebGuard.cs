using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Concurrent.Sync;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 自定义的验证
    /// </summary>
    public sealed class WebGuard : WebSecurityAspect
    {
        private IWebGuard _guard = null;

        private Func<Type, IWebGuard> _getGuard = LazyIndexer.Init<Type, IWebGuard>((type) =>
        {
            var guard = Activator.CreateInstance(type) as IWebGuard;
            if (guard == null) throw new TypeMismatchException(type, typeof(IWebGuard));
            return guard;
        });

        public WebGuard(Type guardType)
        {
            SafeAccessAttribute.CheckUp(guardType);
            _guard = _getGuard(guardType);
        }

        public override void Validate(WebPageContext context)
        {
            _guard.Validate(context);
        }
    }
}