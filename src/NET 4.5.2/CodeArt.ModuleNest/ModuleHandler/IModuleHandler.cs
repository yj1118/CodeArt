using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// 每个模块由若干个处理器组成
    /// </summary>
    /// <typeparam name="Q">请求的参数类型</typeparam>
    /// <typeparam name="S">响应的参数类型</typeparam>
    public interface IModuleHandler<Q,S>
        where Q : class
        where S : class
    {
        /// <summary>
        /// 处理某一项事务
        /// </summary>
        S Process(Q request);
    }
}
