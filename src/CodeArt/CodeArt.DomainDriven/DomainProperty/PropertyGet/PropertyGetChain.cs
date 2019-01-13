using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 获取属性值的行为链
    /// </summary>
    internal sealed class PropertyGetChain
    {
        private DomainProperty _property;

        private List<Func<DomainObject, object>> _methods;

        public PropertyGetChain(DomainProperty property)
        {
            _property = property;
            _methods = new List<Func<DomainObject, object>>();
            _methods.Add(LastStep);
        }

        public void AddMethods(IEnumerable<Func<DomainObject, object>> methods)
        {
            _methods.AddRange(methods);
        }

        private object LastStep(DomainObject domainObject)
        {
            return domainObject.GetValueLastStep(_property);
        }

        public object Invoke(DomainObject domainObject, RunContext ctx)
        {
            if (ctx.MethodIndex == -1) ctx.MethodIndex = _methods.Count;


            object value = null;
            try
            {
                ctx.MethodIndex--;
                var method = _methods[ctx.MethodIndex];
                value = method(domainObject);
            }
            catch
            {
                throw;
            }
            finally
            {
                ctx.MethodIndex++;
            }
            return value;
        }



    }
}