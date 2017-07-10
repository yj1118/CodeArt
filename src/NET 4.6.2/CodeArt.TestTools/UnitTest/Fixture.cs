using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;


using CodeArt.Concurrent;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 测试夹具，在单元测试时运行每个测试之前会初始化夹具，运行完每个测试之后，会清空夹具
    /// </summary>
    [SafeAccess]
    public class Fixture
    {

        private ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

        public Fixture()
        {
        }

        public T Get<T>(string name, T defaultValue)
        {
            object value = null;
            if (_data.TryGetValue(name, out value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        public T Get<T>(string name)
        {
            return Get<T>(name, default(T));
        }

        public void Add<T>(string name, T value)
        {
            _data.TryAdd(name, value);
        }

        #region 单值

        /// <summary>
        /// 根据类型在夹具中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            var name = typeof(T).FullName;
            return Get<T>(name, default(T));
        }

        /// <summary>
        /// 根据类型在夹具中增加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Add<T>(T value)
        {
            var name = typeof(T).FullName;
            Add<T>(name, value);
        }

        #endregion

        public void Clear()
        {
            _data.Clear();
        }

    }
}
