using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    public class PropertySetAttribute : PropertyActionAttribute
    {
        public PropertySetAttribute(string methodName)
            : base(methodName)
        {
        }

        public Action<DomainObject, object> GetAction()
        {
            MethodInfo methodInfo = GetMethod();
            return (domainObject, value) =>
            {
                if(this.IgnoreWithRepository)
                {
                    var current = DataContext.ExistCurrent() ? DataContext.Current : null;
                    if (current != null && current.InBuildObject)
                        return; //指示了在仓储构建时不触发行为，所以返回
                }

                if (methodInfo.IsStatic)
                {
                    //静态的版本
                    using (var temp = ArgsPool.Borrow2())
                    {
                        var args = temp.Item;
                        args[0] = domainObject;
                        args[1] = value;
                        methodInfo.Invoke(null, args);
                    }
                }
                else
                {
                    //对象的版本，对象肯定就是owner
                    using (var temp = ArgsPool.Borrow1())
                    {
                        var args = temp.Item;
                        args[0] = value;
                        methodInfo.Invoke(domainObject, args);
                    }
                }
            };
        }



        internal static IEnumerable<Action<DomainObject, object>> GetMethods(Type objType, string propertyName)
        {
            var attributes = DomainProperty.GetAttributes<PropertySetAttribute>(objType, propertyName);

            List<Action<DomainObject, object>> methods = new List<Action<DomainObject, object>>(attributes.Count());
            foreach (var attribute in attributes)
            {
                methods.Add(attribute.GetAction());
            }
            return methods;
        }


    }
}