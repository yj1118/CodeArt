using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 属于值被改变的行为链
    /// </summary>
    internal sealed class PropertyChangedChain
    {
        private DomainProperty _property;

        private List<Action<DomainObject, DomainPropertyChangedEventArgs>> _methods;

        public PropertyChangedChain(DomainProperty property)
        {
            _property = property;
            _methods = new List<Action<DomainObject, DomainPropertyChangedEventArgs>>();
        }

        public void AddMethods(IEnumerable<Action<DomainObject, DomainPropertyChangedEventArgs>> methods)
        {
            _methods.AddRange(methods);
        }

        public int MethodsCount
        {
            get
            {
                return _methods.Count;
            }
        }

        public void Invoke(DomainObject domainObject, DomainPropertyChangedEventArgs value)
        {
            //属性值被改变的行为链直接遍历所有方法并执行
            foreach (var method in _methods)
            {
                method(domainObject, value);
            }
        }



    }
}