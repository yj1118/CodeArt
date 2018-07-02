using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    public class ListAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小长度
        /// </summary>
        public int Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大长度
        /// </summary>
        public int Max
        {
            get;
            set;
        }

        /// <summary>
        /// 是否检查子项
        /// </summary>
        public bool ValidateItem { get; set; }

        public ListAttribute()
        {
            this.Min = 0;
            this.Max = int.MaxValue;
            this.ValidateItem = false;
        }

        public override IPropertyValidator CreateValidator()
        {
            return new ListValidator(this.Min, this.Max, this.ValidateItem);
        }

    }
}