using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    [DerivedClass(typeof(Increment), "{ACCCDE1C-5A62-4D2B-936C-34A7D0888834}")]
    public class Increment : RegionRule
    {
        private static readonly DomainProperty ValueProperty = DomainProperty.Register<int, Increment>("Value");
        [PropertyRepository()]
        [NotEmpty()]
        public int Value { get; private set; }

        /// <summary>
        /// 占的宽度（也就是PadLeft的宽度）
        /// </summary>
        private static readonly DomainProperty WidthProperty = DomainProperty.Register<int, Increment>("Width");
        [PropertyRepository()]
        [NotEmpty()]
        public int Width { get; private set; }

        public Increment(int id, int width)
            : base(id)
        {
            this.Width = width;
            this.OnConstructed();
        }

        [ConstructorRepository]
        public Increment(int id, int value, int width)
            : base(id)
        {
            this.Value = value;
            this.Width = width;
            this.OnConstructed();
        }

        public override string GetCode()
        {
            this.Value++;
            return this.Value.ToString().PadLeft(this.Width, '0');
        }

        #region 空对象
        public class IncrementEmpty : Increment
        {
            public IncrementEmpty()
               :base (0,0)
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

            public new static readonly Increment Empty = new IncrementEmpty();
        }
        #endregion
    }
}
