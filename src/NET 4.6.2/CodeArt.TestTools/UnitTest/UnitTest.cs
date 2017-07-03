using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using CodeArt.AppSetting;
using CodeArt.Runtime;


namespace CodeArt.TestTools
{
    /// <summary>
    /// 所有的单元测试都应该继承该类
    /// 该类的静态构造函数作为测试环境下的入口
    /// </summary>
    [TestClass]
    public class UnitTest
    {
        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }


        protected Fixture Fixture
        {
            get;
            private set;
        }



        public UnitTest()
        {
            this.Fixture = new Fixture();
            ClassInitialize();
        }       


        protected virtual void ClassInitialize() { }


        [TestInitialize()]
        public void TestInitialize()
        {
            try
            {
                AppSession.Initialize();
                this.Fixture.Clear();
                EachTestInitialize();
            }
            catch(Exception ex)
            {
                TestContext.WriteLine(ex.GetCompleteMessage());
                TestContext.WriteLine(ex.GetCompleteStackTrace());
                throw ex;
            }
        }

        /// <summary>
        /// 在运行每个测试之前，会执行该方法
        /// </summary>
        protected virtual void EachTestInitialize()
        {
           
        }


        //在每个测试运行完之后，使用 TestCleanup 来运行代码
        [TestCleanup()]
        public void TestCleanup()
        {
            try
            {
                EachTestCleanup();
                this.Fixture.Clear();
            }
            catch (Exception ex)
            {
                TestContext.WriteLine(ex.GetCompleteMessage());
                TestContext.WriteLine(ex.GetCompleteStackTrace());
            }
            finally
            {
                AppSession.Dispose();
            }
        }

        /// <summary>
        /// 在每个测试运行完之后，会执行该方法
        /// </summary>
        protected virtual void EachTestCleanup()
        {
            
        }

        /// <summary>
        /// 静态构造函数作为测试环境下的入口
        /// 调用了AppInitializer.Initialize
        /// </summary>
        static UnitTest()
        {
            AppInitializer.Initialize();
        }
    }
}
