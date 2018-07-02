using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    [DerivedClass(typeof(Hardcode), "{B9AA9BD0-5ED4-423E-8946-6196BCF34CE2}")]
    public class Hardcode : RegionRule
    {
        private static readonly DomainProperty ContenetProperty = DomainProperty.Register<string, Hardcode>("Contenet");

        /// <summary>
        /// 硬编码的内容
        /// </summary>
        [PropertyRepository()]
        [NotEmpty]
        [StringLength(1, 100)]
        public string Contenet
        {
            get
            {
                return GetValue<string>(ContenetProperty);
            }
            set
            {
                SetValue(ContenetProperty, value);
            }
        }

        [ConstructorRepository()]
        public Hardcode(int id, string content)
            :base(id)
        {
            this.Contenet = content;
            this.OnConstructed();
        }


        public override string GetCode()
        {
            return this.Contenet;
        }

        #region 空对象

        private class HardcodeEmpty : Hardcode
        {
            public HardcodeEmpty()
                : base(0,string.Empty)
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

        public new static readonly Hardcode Empty = new HardcodeEmpty();

        #endregion

    }
}