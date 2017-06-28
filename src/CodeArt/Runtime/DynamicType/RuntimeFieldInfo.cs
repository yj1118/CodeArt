using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Runtime
{
    public class RuntimeFieldInfo : FieldInfo
    {
        private Type _fieldType;
        private string _fieldName;
        private Type _ownerType;
        private Collection<Attribute> _attributes;

        public RuntimeFieldInfo(string fieldName, Type fieldType, Type ownerType)
        {
            _fieldName = fieldName;
            _fieldType = fieldType;
            _ownerType = ownerType;
            _attributes = new Collection<Attribute>();
        }

        public void AddAttribute(Attribute attribute)
        {
            _attributes.Add(attribute);
        }

        public override RuntimeFieldHandle FieldHandle => throw new NotImplementedException();

        public override Type FieldType => _fieldType;

        public override FieldAttributes Attributes => throw new NotImplementedException();

        public override string Name => _fieldName;

        public override Type DeclaringType => _ownerType;

        public override Type ReflectedType => _ownerType;

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

        public override object GetValue(object obj)
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

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {

        }
    }

    internal class RuntimeFieldInfo<T> : RuntimeFieldInfo
    {
        public RuntimeFieldInfo(string fieldName, Type ownerType)
            : base(fieldName, typeof(T), ownerType)
        {
        }
    }

}
