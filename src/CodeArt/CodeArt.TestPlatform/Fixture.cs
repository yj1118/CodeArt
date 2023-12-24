using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;


using CodeArt.Concurrent;
using CodeArt.DTO;
using System.IO;

namespace CodeArt.TestPlatform
{
    /// <summary>
    /// 
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

        public bool Contains(string name)
        {
            return _data.ContainsKey(name);
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

        public void Remove(string name)
        {
            _data.TryRemove(name, out var value);
        }

        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// 全局夹具
        /// </summary>
        public static readonly Fixture Global = new Fixture();

        /// <summary>
        /// 外部提供的夹具数据
        /// </summary>
        public static readonly DTObject External = DTObject.Empty;


        static Fixture()
        {
            External = LoadExternal().AsReadOnly();
        }

        private static DTObject LoadExternal()
        {
            var json = File.ReadAllText("fixture.json");
            json = json.Replace("\r\n", string.Empty);
            return DTObject.Create(json);
        }

    }
}
