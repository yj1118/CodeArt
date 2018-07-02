using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArtTest.Runtime
{
    /// <summary>
    /// 常规测试
    /// </summary>
    [TestClass]
    public class DynamicMethodCommon
    {
        public DynamicMethodCommon()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DeclareInt()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            IVariable count = g.Declare(3);
            g.Load(count);
            g.Return();
            int result = (int)method.Invoke(null, new object[]{});
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void DeclareString()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(string), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            IVariable count = g.Declare("哈哈哈哈哈");
            g.Load(count);
            g.Return();
            string result = (string)method.Invoke(null, new object[] { });
            Assert.AreEqual("哈哈哈哈哈", result);
        }

        [TestMethod]
        public void If_Condition_And()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            var a = g.Declare(5);

            g.If(() =>{//a<10 && a>6
                g.And(() =>{
                        g.Load(a);
                        g.Load(10);
                        return LogicOperator.LessThan;
                    },() =>{
                        g.Load(a);
                        g.Load(6);
                        return LogicOperator.GreaterThan;
                    });
                return LogicOperator.IsTrue;
            },()=>{
                g.Increment(a);
            });
            g.Load(a);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void If_Condition_Or()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            var a = g.Declare(2);

            g.If(() =>
            {//a>6 || a<1
                g.Or(() =>
                {
                    g.Load(a);
                    g.Load(6);
                    return LogicOperator.GreaterThan;
                }, () =>
                {
                    g.Load(a);
                    g.Load(1);
                    return LogicOperator.LessThan;
                });
                return LogicOperator.IsTrue;
            }, () =>
            {
                g.Increment(a);
            });
            g.Load(a);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(2, result);
        }


        [TestMethod]
        public void While()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            const int maxNumber = 15;
            var num = g.Declare(0);
            var max = g.Declare(maxNumber);
            g.While(() =>
            {
                g.Load(num);
                g.Load(max);
                return LogicOperator.LessThan;
            }, () =>
            {
                g.Increment(num);
            });
            g.Load(num);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(maxNumber, result);
        }

        [TestMethod]
        public void IntToObject()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(object), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(1);
            g.Cast(typeof(object));
            g.Return();
            object result = (object)method.Invoke(null, new object[] { });
            Assert.AreEqual(1, result);
        }



    }
}
