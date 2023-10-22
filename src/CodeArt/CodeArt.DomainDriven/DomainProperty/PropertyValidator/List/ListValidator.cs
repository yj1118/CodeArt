using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    ///  集合验证器
    /// </summary>
    [SafeAccess]
    public class ListValidator : PropertyValidator<IList>
    {
        /// <summary>
        /// 集合的最小长度
        /// </summary>
        public int Min { get; private set; }

        /// <summary>
        /// 集合的最大长度
        /// </summary>
        public int Max { get; private set; }

        /// <summary>
        /// 是否检查子项
        /// </summary>
        public bool ValidateItem { get; private set; }

        public ListValidator(int min, int max,bool validateItem)
        {
            this.Min = min;
            this.Max = max;
            this.ValidateItem = validateItem;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, IList propertyValue, ValidationResult result)
        {
            var list = propertyValue as IList;

            if (list != null) //为null的情况不检查，交由别的对象检查
            {
                int count = list.Count;
                if (count < this.Min) result.AddError(property.Name, ListCountError, string.Format(Strings.ListCountLessThan, property.Call, this.Min));
                else if (count > this.Max) result.AddError(property.Name, ListCountError, string.Format(Strings.ListCountMoreThan, property.Call, this.Max));

                if (this.ValidateItem)
                {
                    foreach (var item in list)
                    {
                        ISupportFixedRules support = item as ISupportFixedRules;
                        if (support != null)
                        {
                            ValidationResult t = support.Validate();
                            if (!t.IsSatisfied) result.AddError(property.Name, ListItemError, string.Format(Strings.ListItemError, property.Call, t.Message));
                        }
                    }
                }
            }
        }

        public const string ListCountError = "ListCountError";
        public const string ListItemError = "ListItemError";

    }
}
