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

        private int _runIndex;

        public PropertySetChain(DomainProperty property)
        {
            _property = property;
            _methods = new List<Action<DomainObject, object>>();
            _methods.Add(LastStep);
            _runIndex = _methods.Count;
        }

        public void AddMethods(IEnumerable<Action<DomainObject, object>> methods)
        {
            _methods.AddRange(methods);
            _runIndex = _methods.Count;
        }

        private void LastStep(DomainObject domainObject, object value)
        {
            domainObject.SetValueLastStep(_property, value);
        }

        public void Invoke(DomainObject domainObject, object value)
        {
            try
            {
                _runIndex--;
                var method = _methods[_runIndex];
                method(domainObject, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _runIndex++;
            }
        }



    }
}