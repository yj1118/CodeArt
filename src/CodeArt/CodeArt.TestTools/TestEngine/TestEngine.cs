using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.TestTools
{
    public static class TestEngine
    {
        /// <summary>
        /// 运行全部测试
        /// </summary>
        public static void RunAll(long userId)
        {
            TestRunner.Run(userId, (runner) =>
            {
                var classKeys = TestCaseAttribute.GetDocumentKeys();
                foreach (var classKey in classKeys)
                {
                    var methods = TestCaseAttribute.GetTestCases(classKey);
                    Run(userId, runner, methods);
                }
            });
            
        }

        public static void RunClass(long userId,string classKey)
        {
            TestRunner.Run(userId, (runner) =>
            {
                var methods = TestCaseAttribute.GetTestCases(classKey);
                Run(userId, runner, methods);
            });
        }

        public static void RunClass(long userId, IEnumerable<string> classKeys)
        {
            TestRunner.Run(userId, (runner) =>
            {
                foreach (var classKey in classKeys)
                {
                    var methods = TestCaseAttribute.GetTestCases(classKey);
                    Run(userId, runner, methods);
                };
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodKeys">测试方法的编号</param>
        public static void Run(long userId, IEnumerable<string> methodKeys)
        {
            TestRunner.Run(userId, (runner) =>
            {
                Run(userId, runner, methodKeys.Select((key) => TestCaseAttribute.GetTestCase(key)));
            });
        }

        private static void Run(long userId, TestRunner runner, IEnumerable<TestCaseAttribute> attrs)
        {
            foreach (var attr in attrs)
            {
                object instance = runner.GetDocumentInstance(attr);
                var method = attr.Method;

                runner.WriteLog(attr, string.Format("正在运行 - " + method.Name), RunStatus.Ing);
                try
                {
                    method.Invoke(instance, Array.Empty<object>());
                    if (runner.IsStop) break;
                }
                catch (Exception ex)
                {
                    runner.WriteLog(attr, ex.GetCompleteInfo(), RunStatus.Error);
                    throw ex;
                }
                runner.WriteLog(attr, string.Format("测试通过 - " + method.Name), RunStatus.Success);
            }
        }


        #region 获取运行时信息

        /// <summary>
        /// 获取运行时的日志
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ticks">时间，如果为0，表示获取所有日志，否则获取ticks以后的日志</param>
        public static TestRuntime GetRuntime(long userId, long ticks)
        {
            return TestRunner.GetRuntime(userId, ticks);
        }

        public static void Stop(long userId)
        {
            TestRunner.Stop(userId);
        }

        #endregion


        /// <summary>
        /// 获得测试类的信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestDocumentAttribute> GetDocuments()
        {
            return TestDocumentAttribute.GetDocuments();
        }

        public static IEnumerable<DTObject> GetDTODocuments()
        {
            return GetDocuments().Select(t => DTObject.Create("{name,key}", t));
        }

        public static IEnumerable<TestCaseAttribute> GetCases(string classKey)
        {
            return TestCaseAttribute.GetTestCases(classKey);
        }

        /// <summary>
        /// 得到测试方法所属的测试类的信息
        /// </summary>
        /// <param name="methodKey"></param>
        /// <returns></returns>
        public static TestDocumentAttribute GetDocument(string methodKey)
        {
            var method = TestCaseAttribute.GetTestCase(methodKey);

            if (method == null)
                return null;

            return method.Document;
        }

        /// <summary>
        /// 根据方法Key得到测试方法的信息
        /// </summary>
        /// <param name="methodKey"></param>
        /// <returns></returns>
        public static TestCaseAttribute GetCase(string methodKey)
        {
            var method = TestCaseAttribute.GetTestCase(methodKey);
            return method;
        }
    }

}
