using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Q">请求的参数类型</typeparam>
    /// <typeparam name="S">响应的参数类型</typeparam>
    public abstract class ModuleHandlerQuery<Q> : ModuleHandlerBase<Q,DTObject>
        where Q : class
    {
        public override DTObject Process(Q request)
        {
            DTObject arg = DTObject.Serialize(request, false);
            return Load(arg);
        }

        protected abstract DTObject Load(DTObject arg);

    }

    public abstract class ModuleHandlerQuery : ModuleHandlerQuery<DTObject>
    {
    }

}
