using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyActionAttribute : Attribute
    {
        /// <summary>
        /// 从仓储中加载对象时，忽略该属性行为
        /// </summary>
        public bool IgnoreWithRepository
        {
            get;
            set;
        }

        public Type ObjectOrExtensionType
        {
            get;
            internal set;
        }

        public string MethodName
        {
            get;
            private set;
        }

        public PropertyActionAttribute(string methodName)
        {
            this.IgnoreWithRepository = true;
            this.MethodName = methodName;
        }

        protected MethodInfo GetMethod()
        {
            return ExtendedClassAttribute.IsObjectExtension(this.ObjectOrExtensionType) ? GetMethodFromObjectExtension() : GetMethodFromObject();
        }

        /// <summary>
        /// 从对象扩展上获得静态方法
        /// </summary>
        /// <param name="ownerType"></param>
        /// <param name="methodName"></param>
        private MethodInfo GetMethodFromObjectExtension()
        {
            var ownerType = this.ObjectOrExtensionType;
            var methodInfo = ownerType.ResolveMethod(this.MethodName);
            ArgumentAssert.IsNotNull(methodInfo, this.MethodName);
            if(!methodInfo.IsStatic)
            {
                throw new DomainDrivenException(string.Format(Strings.PropertyActionObjectExtensionNoStatic, this.MethodName));
            }
            return methodInfo;
        }

        /// <summary>
        /// 从领域对象上获得方法
        /// </summary>
        /// <param name="ownerType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        private MethodInfo GetMethodFromObject()
        {
            var ownerType = this.ObjectOrExtensionType;
            var methodInfo = ownerType.ResolveMethod(this.MethodName);
            ArgumentAssert.IsNotNull(methodInfo, this.MethodName);
            return methodInfo;
        }


    }
}