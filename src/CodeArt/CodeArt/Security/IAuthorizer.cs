using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Security
{
    /// <summary>
    /// 授权器
    /// </summary>
    public interface IAuthorizer<T> where T : AuthAttribute
    {
        bool Verify(T attr, DTObject arg);
    }
}
