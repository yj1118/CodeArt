using System;
using System.Linq;

using CodeArt.Util;

namespace CodeArt.Runtime
{
    internal struct ConstructorKey
    {
        public Type ObjectType
        {
            get;
            private set;
        }


        public Type[] GenericTypes
        {
            get;
            private set;
        }


        public MethodParameter[] Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否为泛型版本（即没有定义实际类型的泛型方法，例如 Add{T}(T obj)
        /// </summary>
        public bool IsGenericVersion
        {
            get;
            private set;
        }

        public ConstructorKey(Type objectType, Type[] genericTypes, MethodParameter[] parameters, bool isGenericVersion)
        {
            this.ObjectType = objectType;
            this.GenericTypes = genericTypes;
            this.Parameters = parameters;
            this.IsGenericVersion = isGenericVersion;
        }

        public override int GetHashCode()
        {
            return HashCoder<MethodKey>.Combine(HashCoder<Type>.GetCode(this.ObjectType)
                                                , HashCoder<Type>.GetCode(this.GenericTypes)
                                                , HashCoder<MethodParameter>.GetCode(this.Parameters)
                                                , HashCoder<bool>.GetCode(this.IsGenericVersion));
        }

        public override bool Equals(object obj)
        {
            if(obj is ConstructorKey)
            {
                var target = (ConstructorKey)obj;
                return EqualsHelper.ObjectEquals(this.ObjectType, target.ObjectType)
                            && this.IsGenericVersion == target.IsGenericVersion
                            && EqualsHelper.ListEquals(this.Parameters, target.Parameters)
                            && EqualsHelper.ListEquals(this.GenericTypes, target.GenericTypes);
            }
            return false;
        }
        /// <summary>
        /// 仅根据名称匹配
        /// </summary>
        public bool IsOnlyMathName
        {
            get
            {
                return this.GenericTypes.Length == 0 && this.Parameters.Length == 0;
            }
        }
    }
}
