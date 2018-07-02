using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    [DerivedClass(typeof(IncrementByMonth),"{9C84F324-5CCD-406B-9A45-8E78054A6F51}")]
    public class IncrementByMonth : RegionRule
    {
        private static readonly DomainProperty MontyProperty = DomainProperty.Register<int, IncrementByMonth>("Month");
        /// <summary>
        /// 当前自增对应的月份
        /// </summary>
        [PropertyRepository()]
        [NotEmpty()]
        [IntRange(1,12)]
        public int Month
        {
            get;
            private set;
        }

        private static readonly DomainProperty ValueProperty = DomainProperty.Register<int, IncrementByMonth>("Value");
        [PropertyRepository()]
        [NotEmpty()]
        public int Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 占的宽度（也就是PadLeft的宽度）
        /// </summary>
        private static readonly DomainProperty WidthProperty = DomainProperty.Register<int, IncrementByMonth>("Width");
        [PropertyRepository()]
        [NotEmpty()]
        public int Width
        {
            get;
            private set;
        }


        public IncrementByMonth(int id,int width)
            :base(id)
        {
            Reset();
            this.Width = width;
            this.OnConstructed();
        }

        private void Reset()
        {
            this.Month = DateTime.Now.Month;
            this.Value = 0;
        }

        /// <summary>
        /// 该构造函数提供给仓储使用，仓储会读取正确的参数赋值对象
        /// </summary>
        /// <param name="month"></param>
        /// <param name="value"></param>
        [ConstructorRepository()]
        internal IncrementByMonth(int id, int month, int value,int width)
            : base(id)
        {
            this.Month = month;
            this.Value = value;
            this.Width = width;
            this.OnConstructed();
        }


        public override string GetCode()
        {
            var currentMonth = DateTime.Now.Month;
            if (currentMonth != this.Month)
            {
                Reset(); //转月了，重置数据
            }

            this.Value++;

            return this.Value.ToString().PadLeft(this.Width, '0');
        }

        #region 空对象
        public class IncrementByMonthEmpty : IncrementByMonth
        {
            public IncrementByMonthEmpty()
                : base(0,0)
                {
                this.OnConstructed();
                }

            public override string GetCode()
            {
                return string.Empty;
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }
        public new static readonly IncrementByMonth Empty = new IncrementByMonthEmpty();

        #endregion
    }
}
