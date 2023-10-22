using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示对象是框架提供的领域对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FrameworkDomainAttribute : Attribute
    {
        /// <summary>
        /// 表示对象可以仓储，仓储的接口类型为所在聚合根的仓储的类型
        /// </summary>
        public FrameworkDomainAttribute()
        {
        }
    }
}
