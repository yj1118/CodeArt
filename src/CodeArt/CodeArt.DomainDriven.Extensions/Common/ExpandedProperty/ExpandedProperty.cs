using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 扩展属性，目前不支持自动验证，以后实现todo
    /// </summary>
    [ObjectValidator()]
    public class ExpandedProperty : ValueObject
    {
        [PropertyRepository()]
        [NotEmpty]
        [StringLength(1, 100)]
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, ExpandedProperty>("Name");

        /// <summary>
        /// 地理位置的名称
        /// </summary>
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        [PropertyRepository()]
        [StringLength(0, 100)]
        public static readonly DomainProperty ValueProperty = DomainProperty.Register<string, ExpandedProperty>("Value");


        public string Value
        {
            get
            {
                return GetValue<string>(ValueProperty);
            }
            private set
            {
                SetValue(ValueProperty, value);
            }
        }

        [ConstructorRepository]
        public ExpandedProperty(string name,string value)
        {
            this.Name = name;
            this.Value = value;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }

        private class ExpandedPropertyEmpty : ExpandedProperty
        {
            public ExpandedPropertyEmpty()
                : base(string.Empty,string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly ExpandedProperty Empty = new ExpandedPropertyEmpty();

    }
}
