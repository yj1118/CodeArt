using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public abstract class PropertyValidatorAttribute : Attribute
    {
        public abstract IPropertyValidator CreateValidator();


        #region 静态方法

        /// <summary>
        /// 根据属性标记的验证器特性，得到验证器
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<IPropertyValidator> GetValidators(Type objType, string propertyName)
        {
            var attributes = DomainProperty.GetAttributes<PropertyValidatorAttribute>(objType, propertyName);

            List<IPropertyValidator> validators = new List<IPropertyValidator>(attributes.Count());
            foreach (var attr in attributes)
            {
                validators.Add(attr.CreateValidator());
            }
            return validators;
        }

        #endregion
    }
}