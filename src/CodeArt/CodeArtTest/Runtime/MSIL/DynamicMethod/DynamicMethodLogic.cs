using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArtTest.Runtime
{
    /// <summary>
    /// PoolingTesting 的摘要说明
    /// </summary>
    [TestClass]
    public class DynamicMethodLogic
    {
        public DynamicMethodLogic()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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
        public void IsFalse_True()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(false);
            g.Compare(LogicOperator.IsFalse);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void IsFalse_False()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(true);
            g.Compare(LogicOperator.IsFalse);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void NotEqual_True()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(2);
            g.Load(3);
            g.Compare(LogicOperator.AreNotEqual);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NotEqual_False()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(2);
            g.Load(2);
            g.Compare(LogicOperator.AreNotEqual);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(0, result);
        }



        [TestMethod]
        public void GreaterThanOrEqualTo_True()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(5);
            g.Load(5);
            g.Compare(LogicOperator.GreaterThanOrEqualTo);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GreaterThanOrEqualTo_False()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(2);
            g.Load(3);
            g.Compare(LogicOperator.GreaterThanOrEqualTo);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(0, result);
        }


        [TestMethod]
        public void LessThan_True()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(2);
            g.Load(3);
            g.Compare(LogicOperator.LessThan);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LessThan_False()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);
            g.Load(3);
            g.Load(2);
            g.Compare(LogicOperator.LessThan);
            g.Return();
            int result = (int)method.Invoke(null, new object[] { });
            Assert.AreEqual(0, result);
        }

        #region if

        [TestMethod]
        public void If_Else()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), Type.EmptyTypes);
            MethodGenerator g = new MethodGenerator(method);

            var count = g.Declare<int>();

            g.If(() =>
            {
                g.Load(true);
                return LogicOperator.IsTrue;
            }, () =>
            {
                g.Assign(count, () =>
                {
                    g.Load(1);
                });
            }, () =>
            {
                g.Assign(count, () =>
                {
                    g.Load(2);
                });
            });
            g.Load(count);
            g.Return();


            int result = (int)method.Invoke(null, null);
            Assert.AreEqual(1, result);
        }

        #endregion


        #region foreach


        [TestMethod]
        public void Foreach()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), new Type[] { typeof(List<int>) });
            MethodGenerator g = new MethodGenerator(method);

            List<int> list = new List<int> { 1,2,3,4,5 };

            g.LoadParameter(0);
            var count =  g.Declare<int>();

            g.ForEach(item =>
            {
                g.Load(item);
                g.Store(count);
            });
            g.Load(count);

            g.Return();

            var result = (int)method.Invoke(null, new object[] { list });
            Assert.AreEqual(5, result);
        }

        #endregion

        #region foreach


        [TestMethod]
        public void For()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), new Type[] { typeof(List<int>) });
            MethodGenerator g = new MethodGenerator(method);

            IVariable array = g.Declare<List<int>>();
            g.Assign(array,()=>
            {
                g.LoadParameter(0);
            });

            var count = g.Declare<int>();
            var number = g.Declare(0);
            g.Assign(count,()=>{
                g.LoadParameter(0);
                g.LoadMember("Count");
            });

            g.For(count, (index) =>
            {
                g.Assign(number, () =>
                {
                    g.LoadParameter(0);
                    g.Load(index);
                    g.Call(typeof(List<int>).ResolveMethod("get_Item", typeof(int)));
                    g.Load(number);
                    g.Add<int>();
                });
            });

            g.Load(number);

            g.Return();

            List<int> list = new List<int> { 1, 2, 3, 4, 5 };
            var result = (int)method.Invoke(null, new object[] { list });
            Assert.AreEqual(15, result);
        }

        #endregion


        [TestMethod]
        public void CallNonStatic()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), new Type[] { typeof(SampleCls) });
            MethodGenerator g = new MethodGenerator(method);
            
            g.Call(typeof(SampleCls).ResolveMethod("Increase",typeof(int)),()=>
                    {
                        g.LoadParameter(0);
                        g.Load(5);
                    });
            g.Return();

            
            int result = (int)method.Invoke(null, new object[] { new SampleCls() });
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void CallStatic()
        {
            DynamicMethod method = new DynamicMethod("temp", typeof(int), new Type[] { typeof(SampleCls) });
            MethodGenerator g = new MethodGenerator(method);

            g.Call(typeof(SampleCls).ResolveMethod("IncreaseStatic", typeof(int)), () =>
            {
                g.Load(5);
            });
            g.Return();


            int result = (int)method.Invoke(null, new object[] { new SampleCls() });
            Assert.AreEqual(5, result);
        }



        public sealed class SampleCls
        {
            private int _count = 0;

            public SampleCls()
            {

            }

            public int Increase(int value)
            {
                _count += value;
                return _count;
            }

            public static int IncreaseStatic(int value)
            {
                return value;
            }

        }


    }
}
