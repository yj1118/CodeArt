using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 设置属性值的行为链
    /// </summary>
    internal sealed class PropertySetChain
    {
        private DomainProperty _property;

        private List<Action<DomainObject, object>> _methods;

        public PropertySetChain(DomainProperty property)
        {
            _property = property;
            _methods = new List<Action<DomainObject, object>>();
            _methods.Add(LastStep);
        }

        public void AddMethods(IEnumerable<Action<DomainObject, object>> methods)
        {
            _methods.AddRange(methods);
        }

        private void LastStep(DomainObject domainObject, object value)
        {
            domainObject.SetValueLastStep(_property, value);
        }

        internal void Invoke(DomainObject domainObject, object value, RunContext ctx)
        {
            if (ctx.MethodIndex == -1) ctx.MethodIndex = _methods.Count;

            try
            {
                ctx.MethodIndex--;
                var method = _methods[ctx.MethodIndex];
                method(domainObject, value);
            }
            catch
            {
                throw;
            }
            finally
            {
                ctx.MethodIndex++;
            }
        }
    }
}