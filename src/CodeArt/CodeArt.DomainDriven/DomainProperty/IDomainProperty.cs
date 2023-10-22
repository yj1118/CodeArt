using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        string Name { get; }

        string Call { get; }

        /// <summary>
        /// 属性的类型
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// 拥有该属性的类型
        /// </summary>
        Type OwnerType { get;}

        ///// <summary>
        ///// 属性更改模式
        ///// </summary>
        //PropertyChangedMode ChangedMode { get;  }


        /// <summary>
        /// 获得属性的默认值,传入属性所在的对象和成员对应的属性定义
        /// </summary>
        /// <returns></returns>
        Func<DomainObject, DomainProperty, object> GetDefaultValue { get; }



        /// <summary>
        /// 属性的验证器
        /// </summary>
        IEnumerable<IPropertyValidator> Validators { get; }

    }
}
