using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

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
        public object OldValue { get; internal set; }
        //
        // 摘要:
        //     获取发生值更改的领域项属性的标识符。
        //
        // 返回结果:
        //     发生值更改的领域项属性的标识符字段。
        public DomainProperty Property { get; internal set; }

        private DomainPropertyChangedEventArgs() { }

        internal void Reset()
        {
            this.NewValue = null;
            this.OldValue = null;
            this.Property = null;
        }

        private static Pool<DomainPropertyChangedEventArgs> _pool = new Pool<DomainPropertyChangedEventArgs>(() =>
        {
            return new DomainPropertyChangedEventArgs();
        }, (args, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                args.Reset();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });

        public static IPoolItem<DomainPropertyChangedEventArgs> Borrow(DomainProperty property, object newValue, object oldValue)
        {
            var temp = _pool.Borrow();
            var args = temp.Item;
            args.Property = property;
            args.NewValue = newValue;
            args.OldValue = oldValue;
            return temp;
        }
    }
}
