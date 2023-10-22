using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Security;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 授权过滤器
    /// </summary>
    public interface IAuthFilter
    {
        /// <summary>
        /// 是否忽略授权（业务级授权）
        /// </summary>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Ignore(string scope, DTObject data);
    }
}
