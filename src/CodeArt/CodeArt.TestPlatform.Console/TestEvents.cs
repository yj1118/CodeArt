using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.TestPlatform
{
    public static class TestEvents
    {
        #region 测试的开始与结束

        /// <summary>
        /// RPC服务器已打开的事件
        /// </summary>
        public class TestSceneArgs : EventArgs
        {
            public TestSceneAttribute Tip
            {
                get;
                private set;
            }

            public TestSceneArgs(TestSceneAttribute tip)
            {
                this.Tip = tip;
            }
        }

        public delegate void TestStartHandler(object sender, TestSceneArgs arg);

        public static event TestStartHandler TestStart;

        internal static void RaiseTestStart(object sender, TestSceneArgs arg)
        {
            if (TestStart != null)
            {
                TestStart(sender, arg);
            }
        }

        public delegate void TestEndHandler(object sender, TestSceneArgs arg);

        public static event TestEndHandler TestEnd;

        internal static void RaiseTestEnd(object sender, TestSceneArgs arg)
        {
            if (TestEnd != null)
            {
                TestEnd(sender, arg);
            }
        }

        #endregion


        #region 测试用例的开始与结束


        public class TestCaseArgs : EventArgs
        {
            public string CaseName
            {
                get;
                private set;
            }

            public TestCaseAttribute Tip
            {
                get;
                private set;
            }

            public TestContext Context
            {
                get;
                private set;
            }

            public TestCaseArgs(string caseName, TestCaseAttribute tip, TestContext context)
            {
                this.CaseName = caseName;
                this.Tip = tip;
                this.Context = context;
            }
        }

        public delegate void TestCaseStartHandler(object sender, TestCaseArgs arg);

        public static event TestCaseStartHandler TestCaseStart;

        internal static void RaiseTestCaseStart(object sender, TestCaseArgs arg)
        {
            if (TestCaseStart != null)
            {
                TestCaseStart(sender, arg);
            }
        }


        public delegate void TestCaseEndHandler(object sender, TestCaseArgs arg);

        public static event TestCaseEndHandler TestCaseEnd;

        internal static void RaiseTestCaseEnd(object sender, TestCaseArgs arg)
        {
            if (TestCaseEnd != null)
            {
                TestCaseEnd(sender, arg);
            }
        }

        #endregion


        #region 测试过程里发生异常的事件


        public class TestErrorArgs : EventArgs
        {
            public Exception Error
            {
                get;
                private set;
            }

            public TestErrorArgs(Exception error)
            {
                this.Error = error;
            }
        }

        public delegate void TestErrorHandler(object sender, TestErrorArgs arg);

        public static event TestErrorHandler TestError;

        internal static void RaiseTestError(object sender, TestErrorArgs arg)
        {
            if (TestError != null)
            {
                TestError(sender, arg);
            }
        }

        #endregion
    }
}
