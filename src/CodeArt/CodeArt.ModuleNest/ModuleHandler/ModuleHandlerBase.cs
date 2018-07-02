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
    public abstract class ModuleHandlerBase<Q,S> : IModuleHandler<Q,S>
        where Q : class
        where S : class
    {
        /// <summary>
        /// 处理某一项事务
        /// </summary>
        public abstract S Process(Q request);
    }

    public abstract class ModuleHandlerBase : ModuleHandlerBase<DTObject, DTObject>
    {
        public override DTObject Process(DTObject request)
        {
            return DynamicInvoke((dynamic)request);
        }

        protected virtual DTObject DynamicInvoke(dynamic arg)
        {
            return DTObject.Empty;
        }

    }

}
