using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.DTO;
using CodeArt.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeArt.TestPlatform.VisualStudio
{
    public class TestScene
    {

        ///// <summary>
        ///// 整个场景被执行之前进行的初始化操作
        ///// </summary>
        //[ClassInitialize]
        //public static void SceneInitialize(TestContext context)
        //{

        //}

        //[ClassCleanup]
        //public static void SceneCleanup()
        //{

        //}

        public static Fixture Fixture
        {
            get;
            private set;
        }

        static TestScene()
        {
            Fixture = new Fixture();
        }



        public TestScene()
        {

        }


        /// <summary>
        /// 每个测试用例被执行之前的初始化操作
        /// </summary>
        [TestInitialize]
        public void _EachCaseInitialize()
        {
            EachCaseInitialize();
        }

        protected virtual void EachCaseInitialize()
        {

        }

        /// <summary>
        /// 每个测试用例被执行之后的清理操作
        /// </summary>
        [TestCleanup]
        public void _EachCaseCleanup()
        {
            EachCaseCleanup();
        }

        protected virtual void EachCaseCleanup()
        {

        }


    }
}