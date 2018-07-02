using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.DomainDriven;

namespace SerialNumberSubsystem
{
    /// <summary>
    /// 根据当前日期生成编码
    /// </summary>
    [DerivedClass(typeof(DateCode), "{7BC28BB4-474E-476A-97DE-47D62C7C78C9}")]
    public class DateCode : RegionRule
    {
        [ConstructorRepository()]
        public DateCode(int id)
            : base(id)
        {
            this.OnConstructed();
        }


        public override string GetCode()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        #region 空对象
        private class DateCodeEmpty : DateCode
        {
            public DateCodeEmpty()
                :base(0)
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
        public new static readonly DateCode Empty = new DateCodeEmpty();
        #endregion

    }
}