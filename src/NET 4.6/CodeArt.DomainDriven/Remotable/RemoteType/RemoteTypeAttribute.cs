using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示对象是远程类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RemoteTypeAttribute : Attribute
    {
        /// <summary>
        /// 对象的远程命名空间
        /// </summary>
        public string TypeNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// 对象的远程类型名称
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        public RemoteTypeAttribute()
        {
        }
    }
}
