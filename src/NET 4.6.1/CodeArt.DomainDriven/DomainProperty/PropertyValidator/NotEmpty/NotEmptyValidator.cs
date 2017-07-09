using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public class NotEmptyValidator : PropertyValidator
    {
        private NotEmptyValidator()
        {
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
                if(string.IsNullOrEmpty(stringValue))
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
            result.AddError(property.Name, "NotEmpty", string.Format(Strings.NotEmpty, property.Name));
        }

        public const string ErrorCode = "NotEmpty";

        public static readonly NotEmptyValidator Instance = new NotEmptyValidator();

    }
}
