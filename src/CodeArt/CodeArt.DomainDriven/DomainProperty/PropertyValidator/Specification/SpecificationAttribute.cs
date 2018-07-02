using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指定属性是一个需要符合规格的对象
    /// </summary>
    public class SpecificationAttribute : PropertyValidatorAttribute
    {
        
        public SpecificationAttribute()
        {
        }

        public override IPropertyValidator CreateValidator()
        {
            return SpecificationValidator.Instance;
        }
    }
}