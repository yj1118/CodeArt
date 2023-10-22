using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public class NotEmptyValidator : PropertyValidator
    {
        /// <summary>
        /// 是否过滤空格，默认情况下过滤
        /// </summary>
        public bool Trim
        {
            get;
            private set;
        }

        public NotEmptyValidator(bool trim)
        {
            this.Trim = trim;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, object propertyValue, ValidationResult result)
        {
            if (propertyValue == null)
            {
                AddError(property, result);
                return;
            }

            INotNullObject notNullObj = propertyValue as INotNullObject;
            if (notNullObj != null)
            {
                if (notNullObj.IsEmpty())
                {
                    AddError(property, result);
                }
                return;
            }

            if(property.PropertyType == typeof(string))
            {
                var stringValue = (string)propertyValue;
                if (this.Trim) stringValue = stringValue.Trim();

                if (string.IsNullOrEmpty(stringValue))
                {
                    AddError(property, result);
                }
                return;
            }

            if (property.PropertyType.IsList())
            {
                var listValue = (IList)propertyValue;
                if (listValue.Count == 0)
                {
                    AddError(property, result);
                }
                return;
            }


            if (DataUtil.IsPrimitiveType(property.PropertyType))
            {
                if (DataUtil.IsDefaultValue(propertyValue))
                {
                    AddError(property, result);
                }
                return;
            }
        }

        private void AddError(IDomainProperty property, ValidationResult result)
        {
            result.AddError(property.Name, "NotEmpty", string.Format(Strings.NotEmpty, property.Call));
        }

        public const string ErrorCode = "NotEmpty";
    }
}
