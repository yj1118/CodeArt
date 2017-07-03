using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    internal class FixedRules : IFixedRules
    {
        private FixedRules() { }

        public ValidationResult Validate(IDomainObject obj)
        {
            ValidationResult result = ValidationResult.Create();
            ValidateProperties(obj, result); //先验证属性
            ValidateObject(obj, result);     //再验证对象
            return result;
        }

        #region 验证属性

        private void ValidateProperties(IDomainObject obj, ValidationResult result)
        {
            var properties = DomainProperty.GetProperties(obj.GetType());
            foreach (var property in properties)
            {
                if(obj.IsPropertyDirty(property))
                {
                    //我们只用验证脏属性
                    ValidateProperty(obj, property, result);
                }

            }
        }

        private void ValidateProperty(IDomainObject obj, IDomainProperty property, ValidationResult result)
        {
            foreach (var validator in property.Validators)
            {
                validator.Validate(obj, property, result);
            }
        }

        #endregion

        #region 验证对象

        private void ValidateObject(IDomainObject obj, ValidationResult result)
        {
            foreach (var validator in obj.Validators)
            {
                validator.Validate(obj, result);
            }
        }


        #endregion

        public static readonly FixedRules Instance = new FixedRules();
    }
}