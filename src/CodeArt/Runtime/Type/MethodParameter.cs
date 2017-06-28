using System;
using System.Linq;

using CodeArt.Util;

namespace CodeArt.Runtime
{
    public struct MethodParameter
    {
        public Type Type
        {
            get;
            private set;
        }

        public bool IsGeneric
        {
            get;
            private set;
        }

        internal MethodParameter(Type type, bool isGeneric, bool isByRef)
        {
            this.Type = isByRef ? type.ResolveByRef() : type;
            this.IsGeneric = isGeneric;
        }

        internal MethodParameter(Type type)
            : this(type, false, false)
        {
        }


        public static MethodParameter Create<T>()
        {
            return new MethodParameter(typeof(T), false, false);
        }

        public static MethodParameter Create<T>(bool isByRef)
        {
            return new MethodParameter(typeof(T), false, isByRef);
        }

        public static MethodParameter CreateGeneric<T>()
        {
            return new MethodParameter(typeof(T), true, false);
        }

        public static MethodParameter CreateGeneric<T>(bool isByRef)
        {
            return new MethodParameter(typeof(T), true, isByRef);
        }

        public static MethodParameter CreateGeneric(Type type)
        {
            return new MethodParameter(type, true, false);
        }

        public static MethodParameter CreateGeneric(Type type, bool isByRef)
        {
            return new MethodParameter(type, true, isByRef);
        }

        /// <summary>
        /// 指定一个弱类型的参数
        /// 该参数主要是用于获取泛型通用方法时候使用
        /// 例如：GetCount{T}(T i);
        /// </summary>
        /// <returns></returns>
        public static MethodParameter CreateGeneric()
        {
            return new MethodParameter(typeof(int), true, false);
        }

        public override int GetHashCode()
        {
            return HashCoder<MethodParameter>.Combine(HashCoder<Type>.GetCode(this.Type)
                                                    , HashCoder<bool>.GetCode(this.IsGeneric));
        }

        public override bool Equals(object obj)
        {
            if (obj is MethodParameter)
            {
                var target = (MethodParameter)obj;
                return EqualsHelper.ObjectEquals(this.Type, target.Type)
                            && this.IsGeneric == target.IsGeneric;
            }
            return false;
        }

        public static readonly MethodParameter[] EmptyParameters = Array.Empty<MethodParameter>();
    }
}
