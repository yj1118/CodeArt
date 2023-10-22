using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.Security
{
    /// <summary>
    /// 标记访问对象的权限
    /// </summary>
    public abstract class AuthAttribute : Attribute
    {
        public AuthAttribute()
        {

        }

        public abstract bool Verify(DTObject data);
    }
}

