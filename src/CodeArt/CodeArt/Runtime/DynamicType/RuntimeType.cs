using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class RuntimeType : Type
    {
        private Guid _id;
        private string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">类型名称</param>
        public RuntimeType(string name)
        {
            _id = Guid.NewGuid();
            _name = name;
            _attributes = new Collection<Attribute>();
            _propertyInfos = new Collection<RuntimePropertyInfo>();
            _fieldInfos = new Collection<RuntimeFieldInfo>();
            _interfaceTypes = new Collection<Type>();
        }

        #region 特性

        private Collection<Attribute> _attributes;
        public void AddAttribute(Attribute attribute)
        {
            _attributes.Add(attribute);
        }

        #endregion

        #region 属性信息

        private Collection<RuntimePropertyInfo> _propertyInfos;

        public RuntimePropertyInfo AddProperty<T>(string name)
        {
            var property = new RuntimePropertyInfo<T>(name, this);
            _propertyInfos.Add(property);
            return property;
        }

        public RuntimePropertyInfo AddProperty(string name, Type propertyType)
        {
            var property = new RuntimePropertyInfo(name, propertyType, this);
            _propertyInfos.Add(property);
            return property;
        }


        #endregion

        #region 字段信息

        private Collection<RuntimeFieldInfo> _fieldInfos;

        public RuntimeFieldInfo AddField<T>(string name)
        {
            var field = new RuntimeFieldInfo<T>(name, this);
            _fieldInfos.Add(field);
            return field;
        }

        public RuntimeFieldInfo AddField(string name, Type fieldType)
        {
            var field = new RuntimeFieldInfo(name, fieldType, this);
            _fieldInfos.Add(field);
            return field;
        }


        #endregion

        #region 实现的接口

        private Collection<Type> _interfaceTypes;

        public void AddInterface<T>()
        {
            AddInterface(typeof(T));
        }

        public void AddInterface(Type interfaceType)
        {
            _interfaceTypes.Add(interfaceType);
        }

        #endregion


        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(Type o)
        {
            return _id == o.GUID;
        }

        public override bool Equals(object o)
        {
            var target = o as Type;
            if (target == null) return false;
            return _id == target.GUID;
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override bool IsByRefImpl()
        {
            return true;
        }

        protected override bool IsCOMObjectImpl()
        {
            return false;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

    
        public override Type GetInterface(string name, bool ignoreCase)
        {
            return _interfaceTypes.List.FirstOrDefault((type)=>
            {
                return ignoreCase ? type.FullName.EqualsIgnoreCase(name)
                                  : type.FullName.Equals(name);
            });
        }

        public override Type[] GetInterfaces()
        {
            return _interfaceTypes.Array;
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return _fieldInfos.List.FirstOrDefault((f) =>
            {
                return f.Name.EqualsIgnoreCase(name);
            });
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return _fieldInfos.Array;
        }



        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return _propertyInfos.List.FirstOrDefault((p)=>
            {
                return p.Name.EqualsIgnoreCase(name);
            });
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return _propertyInfos.Array;
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return TypeAttributes.Class;
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
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

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _attributes.List.Where((attr) =>
            {
                return attr.GetType().IsImplementOrEquals(attributeType);
            }).FirstOrDefault() != null;
        }

        public override Guid GUID => _id;

        public override Assembly Assembly => throw new NotImplementedException();

        public override Module Module => throw new NotImplementedException();

        public override string FullName => _name;

        public override string Namespace => "DynamicType";

        public override string AssemblyQualifiedName => throw new NotImplementedException();

        public override Type BaseType => null;

        public override Type UnderlyingSystemType => null;

        public override string Name => _name;

    }

}
