using System;
using System.Collections.Generic;
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
    }
}
