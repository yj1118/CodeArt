using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

namespace CodeArt.EasyMQ
{
    /// <summary>
    /// 保证工厂是优先选择配置文件里的，如果配置文件没有定义工厂，那么使用注册器的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class FactorySetting<T>
    {
        private T _factory;

        public FactorySetting(Func<T> getByConfig)
        {
            _factory = getByConfig();
        }

        public void Register(T factory)
        {
            if (_factory == null)
                _factory = factory;
        }

        public T GetFactory()
        {
            if (_factory == null) throw new EasyMQException(string.Format(Strings.NotFoundFactory, typeof(T).FullName));
            return _factory;
        }
    }
}
