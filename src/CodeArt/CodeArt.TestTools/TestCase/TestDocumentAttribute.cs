using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 标记对象是一个测试用例
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class TestDocumentAttribute : Attribute
    {
        public string Key
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return string.Format("{0} , {1}", this.ClassType.FullName, this.ClassType.Assembly.FullName.Split(',')[0]);
            }
        }

        public Type ClassType
        {
            get;
            private set;
        }

        private void Init(Type classType)
        {
            this.ClassType = classType;
        }

        public TestDocumentAttribute(string key)
        {
            this.Key = key;
        }

        public static IEnumerable<TestDocumentAttribute> GetDocuments()
        {
            return _data.Values.OrderBy((attr) => attr.ClassType.Name);
        }

        public static TestDocumentAttribute GetDocuments(string documentKey)
        {
            if (_data.TryGetValue(documentKey, out var attr)) return attr;
            return null;
        }


        private static TestDocumentAttribute GetDocumentAttribute(Type type)
        {
            return type.GetCustomAttributes(typeof(TestDocumentAttribute), true).OfType<TestDocumentAttribute>().FirstOrDefault();
        }

        private static Dictionary<string, TestDocumentAttribute> _data = new Dictionary<string, TestDocumentAttribute>(GuidComparer.Cache);

        static TestDocumentAttribute()
        {
            //遍历所有类型，找出打上标签的类型
            var types = AssemblyUtil.GetImplementTypes(typeof(ITestDocument)); //通过接口先找出类型，会大大减少运算量
            foreach (var classType in types)
            {
                if (classType.IsAbstract || classType.IsInterface) continue;
                var attr = GetDocumentAttribute(classType);
                if (attr == null) continue;

                var key = attr.Key;
                if (_data.ContainsKey(key))
                    throw new TestException(string.Format("{0} 重复", attr.Key));

                attr.Init(classType);

                _data.Add(key, attr);
            }
        }

    }
}

