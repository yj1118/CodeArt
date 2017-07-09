using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class DomainPropertyChangedEventArgs : IDomainPropertyChangedEventArgs
    {
        //
        // 摘要:
        //     获取领域项属性在报告的更改后的值。
        //
        // 返回结果:
        //     发生更改之后的领域项属性值。
        public object NewValue { get; internal set; }
        //
        // 摘要:
        //     获取领域项属性在报告的更改前的值。
        //
        // 返回结果:
        //     发生更改之前的领域项属性值。
        public object OldValue { get; private set; }
        //
        // 摘要:
        //     获取发生值更改的领域项属性的标识符。
        //
        // 返回结果:
        //     发生值更改的领域项属性的标识符字段。
        public DomainProperty Property { get; private set; }

        public DomainPropertyChangedEventArgs(DomainProperty property, object newValue, object oldValue)
        {
            this.Property = property;
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

    }
}
