using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 有时，对象不是一个事物。在某些情况下，最清楚、最实用的设计会包含一些特殊的操作，这些操作从概念上讲不属于任何对象。
    /// 与其把它们强制地归于哪一类，不如顺其自然地在模型中引入一种新的元素，这就是领域层的Service
    /// </summary>
    public interface IDomainService
    {
    }
}
