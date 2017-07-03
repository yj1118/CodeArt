using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    public class PropertyGetAttribute : PropertyActionAttribute
    {
        public PropertyGetAttribute(string methodName)
            : base(methodName)
        {
        }

        internal PropertyGetAttribute(Func<DomainObject, object> getAction)
            : base(string.Empty)
        {
            _getAction = getAction;
        }

        private Func<DomainObject, object> _getAction;

        public Func<DomainObject, object> GetAction()
        {
            if (_getAction != null) return _getAction; //自定义了方法

            MethodInfo methodInfo = GetMethod();
            return (domainObject) =>
            {
                object value = null;
                if (methodInfo.IsStatic)
                {
                    using (var temp = ArgsPool.Borrow1())
                    {
                        var args = temp.Item;
                        args[0] = domainObject;
                        value = methodInfo.Invoke(null, args);
                    }
                }
                else
                {
                    value = methodInfo.Invoke(domainObject, EmptyArray<object>.Value);
                }
                return value;
            };
        }



        internal static IEnumerable<Func<DomainObject, object>> GetMethods(Type objType, string propertyName)
        {
            var attributes = DomainProperty.GetAttributes<PropertyGetAttribute>(objType, propertyName);

            List<Func<DomainObject, object>> methods = new List<Func<DomainObject, object>>(attributes.Count());
            foreach (var attribute in attributes)
            {
                methods.Add(attribute.GetAction());
            }
            return methods;
        }


    }
}