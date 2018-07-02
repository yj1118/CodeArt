using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 可以为领域对象指定多个类型的固定规则
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectValidatorAttribute : Attribute
    {
        public IEnumerable<IObjectValidator> Validators
        {
            get;
            private set;
        }

        public ObjectValidatorAttribute(params Type[] validatorTypes)
        {
            List<IObjectValidator> validators = new List<IObjectValidator>(validatorTypes.Length);
            foreach(var validatorType in validatorTypes)
            {
                validators.Add(CreateValidator(validatorType));
            }
            this.Validators = validators;
        }

        private IObjectValidator CreateValidator(Type validatorType)
        {
            return SafeAccessAttribute.CreateSingleton<IObjectValidator>(validatorType);
        }


        #region 静态方法

        /// <summary>
        /// 根据对象标记的验证器特性，得到验证器
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<IObjectValidator> GetValidators(Type objectType)
        {
            var attribute = AttributeUtil.GetAttribute<ObjectValidatorAttribute>(objectType);
            if (attribute == null) return Array.Empty<IObjectValidator>();
            return attribute.Validators;
        }

        #endregion

    }
}
