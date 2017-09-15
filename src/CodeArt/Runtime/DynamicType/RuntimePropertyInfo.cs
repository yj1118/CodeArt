using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Runtime
{
    public class RuntimePropertyInfo : PropertyInfo
    {
        private string _propertyName;
        private Type _propertyType;
        private Type _ownerType;
        private Collection<Attribute> _attributes;

        public RuntimePropertyInfo(string propertyName, Type propertyType, Type ownerType)
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
            _ownerType = ownerType;
            _attributes = new Collection<Attribute>();
        }

        public void AddAttribute(Attribute attribute)
        {
            _attributes.Add(attribute);
        }

        public override Type PropertyType => _propertyType;

        public override PropertyAttributes Attributes => PropertyAttributes.None;

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override string Name => _propertyName;

        public override Type DeclaringType => _ownerType;

        public override Type ReflectedType => _ownerType;

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _attributes.Array;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _attributes.List.Where((attr) =>
            {
                return attr.GetType().IsImplementOrEquals(attributeType);
            }).ToArray();
        }


        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return null;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            return Array.Empty<ParameterInfo>();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            return null;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _attributes.List.Where((attr) =>
            {
                return attr.GetType().IsImplementOrEquals(attributeType);
            }).FirstOrDefault() != null;
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            
        }
    }

    internal class RuntimePropertyInfo<T> : RuntimePropertyInfo
    {
        public RuntimePropertyInfo(string propertyName, Type ownerType)
            : base(propertyName, typeof(T), ownerType)
        {
        }
    }
}
