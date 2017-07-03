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

        private int _runIndex;

        public PropertyGetChain(DomainProperty property)
        {
            _property = property;
            _methods = new List<Func<DomainObject, object>>();
            _methods.Add(LastStep);
            _runIndex = _methods.Count;
        }

        public void AddMethods(IEnumerable<Func<DomainObject, object>> methods)
        {
            _methods.AddRange(methods);
            _runIndex = _methods.Count;
        }

        private object LastStep(DomainObject domainObject)
        {
            return domainObject.GetValueLastStep(_property);
        }

        public object Invoke(DomainObject domainObject)
        {
            object value = null;
            try
            {
                _runIndex--;
                var method = _methods[_runIndex];
                value = method(domainObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _runIndex++;
            }
            return value;
        }



    }
}