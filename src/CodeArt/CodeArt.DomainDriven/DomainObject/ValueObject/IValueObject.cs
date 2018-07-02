using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 当我们只关心一个模型的属性时，应把它归类为Value Object。
    /// 我们应该使这个模型元素能够表示出其属性的意义，并为它提供相关功能。
    /// Value Object应该是不可变的。不要为它分配任何标示，而且不要把它设计成像Entity那么复杂。
    /// </summary>
    public interface IValueObject : IDomainObject
    {
        /// <summary>
        /// 由于ORM存储中间表时需要用到编号，所以我们提供了该属性
        /// 该属性仅在ORM中使用，不要在领域层中出现
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Guid Id
        {
            get;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void TrySetId(Guid id);
    }
}
