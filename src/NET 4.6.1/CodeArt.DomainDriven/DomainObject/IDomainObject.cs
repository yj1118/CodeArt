using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 领域对象接口，所有领域对象要实现的基本接口
    /// </summary>
    public interface IDomainObject : INotNullObject, IStateObject, ISupportFixedRules
    {
        /// <summary>
        /// 对象验证器
        /// </summary>
        IEnumerable<IObjectValidator> Validators
        {
            get;
        }

        /// <summary>
        /// 对象是否发生了改变
        /// </summary>
        bool IsChanged
        {
            get;
        }

        /// <summary>
        /// 对象是否为一个快照
        /// </summary>
        bool IsSnapshot
        {
            get;
        }

        /// <summary>
        /// 属性是否发生改变
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        //bool IsPropertyChanged(string propertyName);

        bool IsPropertyChanged(DomainProperty property);

        /// <summary>
        /// 属性还是否为脏的（属性没有发生改变，不一定不是脏的，例如：引用关系没变，属性也没变，但是有可能属性内部的值发生了改变）
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        //bool IsPropertyDirty(string propertyName);

        /// <summary>
        /// 属性还是否为脏的（属性没有发生改变，不一定不是脏的，例如：引用关系没变，属性也没变，但是有可能属性内部的值发生了改变）
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        bool IsPropertyDirty(DomainProperty property);

        /// <summary>
        /// 属性被更改时触发
        /// </summary>
        event DomainPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 对象的数据版本号
        /// </summary>
        int DataVersion
        {
            get;
        }

    }

}
