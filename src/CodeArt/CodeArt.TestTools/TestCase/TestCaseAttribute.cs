using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;
using System.Reflection;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 标记对象是一个测试用例
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class TestCaseAttribute : Attribute
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
                return this.Method.Name;
            }
        }


        public TestDocumentAttribute Document
        {
            get;
            private set;
        }


        public Type DocumentType
        {
            get
            {
                return this.Document.ClassType;
            }
        }


        public MethodInfo Method
        {
            get;
            private set;
        }

        private void Init(TestDocumentAttribute document, MethodInfo method)
        {
            this.Document = document;
            this.Method = method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">测试方法的唯一标示，可以从这个标示里找到该方法，一般用guid标示</param>
        public TestCaseAttribute(string key)
        {
            this.Key = key;
        }


        #region 辅助


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        internal static TestCaseAttribute GetTestCase(string key)
        {
            if(_caseKeyToCases.TryGetValue(key,out var value))
            {
                return value;
            }
            return null;
        }

        internal static IEnumerable<string> GetDocumentKeys()
        {
            return _documentKeyToCases.Keys.OrderBy((key) => key);
        }

        internal static IEnumerable<TestCaseAttribute> GetTestCases(string classKey)
        {
            return _documentKeyToCases.GetValues(classKey).OrderBy((attr) => attr.Method.Name);
        }

        private static IEnumerable<TestCaseAttribute> GetCaseAttributes(TestDocumentAttribute document)
        {
            const BindingFlags flags =
             BindingFlags.DeclaredOnly |
             BindingFlags.Instance |
             BindingFlags.Public |
             BindingFlags.NonPublic |
               BindingFlags.IgnoreCase | BindingFlags.Static;

            return document.ClassType.GetMethods(flags).Select((mi) =>
            {
                var attr = mi.GetCustomAttribute<TestCaseAttribute>();
                if(attr != null) attr.Init(document, mi);
                return attr;
            }).Where((attr) => attr != null);
        }

  
        private static Dictionary<string, TestCaseAttribute> _caseKeyToCases = new Dictionary<string, TestCaseAttribute>(GuidComparer.Cache);

        private static MultiDictionary<string, TestCaseAttribute> _documentKeyToCases = new MultiDictionary<string, TestCaseAttribute>(true, GuidComparer.Cache);

        static TestCaseAttribute()
        {
            //遍历所有类型，找出打上标签的类型
            var documentAttrs = TestDocumentAttribute.GetDocuments();
            foreach (var documentAttr in documentAttrs)
            {
                var attrs = GetCaseAttributes(documentAttr);
                foreach (var attr in attrs)
                {
                    var key = attr.Key;
                    if (_caseKeyToCases.ContainsKey(key))
                        throw new TestException(string.Format("{0} 重复", attr.Key));
                    _caseKeyToCases.Add(key, attr);
                    _documentKeyToCases.Add(documentAttr.Key, attr);
                }
            }
        }


        #endregion

    }
}

