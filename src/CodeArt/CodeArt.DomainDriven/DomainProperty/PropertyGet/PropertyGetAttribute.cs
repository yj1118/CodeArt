using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    public class PropertyGetAttribute : PropertyActionAttribute
    {
        public PropertyGetAttribute(string methodName)
            : base(methodName)
        {
            this.IgnoreWithRepository = false;//get方法在仓储构建对象时不默认不忽略
        }

        internal PropertyGetAttribute(Func<DomainObject, object> getAction)
            : base(string.Empty)
        {
            _getAction = getAction;
        }

        private Func<DomainObject, object> _getAction;

        public Func<DomainObject, object> GetAction()
        {
            MethodInfo methodInfo = GetMethod();
            return (domainObject) =>
            {
                if (this.IgnoreWithRepository)
                {
                    var current = DataContext.ExistCurrent() ? DataContext.Current : null;
                    if (current != null && current.InBuildObject)
                        return domainObject; //指示了在仓储构建时不触发行为，所以返回
                }

                if (_getAction != null) return _getAction(domainObject);

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
                    value = methodInfo.Invoke(domainObject, Array.Empty<object>());
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