using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

using CodeArt.Runtime;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class ResolveMethodTest
    {
        public ResolveMethodTest()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// SampleCls
        /// void GetCount() { }
        /// </summary>
        [TestMethod]
        public void ResolveMethodZeroParamter()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount");
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(0, methodInfo.GetParameters().Length);
        }

        /// <summary>
        /// SampleCls
        /// void GetCount(int i) { }
        /// </summary>
        [TestMethod]
        public void ResolveMethodOneParamter()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new Type[] { typeof(int) });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(1, methodInfo.GetParameters().Length);
        }

        /// <summary>
        /// SampleCls
        /// string GetCount(int i, string name) { return "SampleCls"; }
        /// </summary>
        [TestMethod]
        public void ResolveMethodTwoParamter_Int_String()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new Type[] { typeof(int), typeof(string) });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[0].ParameterType);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[1].ParameterType);
        }

        /// <summary>
        /// SampleCls
        /// string GetCount(string name, int i) { }
        /// </summary>
        [TestMethod]
        public void ResolveMethodTwoParamter_String_Int()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new Type[] { typeof(string), typeof(int) });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[0].ParameterType);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[1].ParameterType);
        }

        /// <summary>
        /// 获取继承类的方法(方法在基类)
        /// SampleInheritCls
        /// void GetCount(string name, int i) { }
        /// </summary>
        [TestMethod]
        public void ResolveMethodTwoParamterInherit_String_Int()
        {
            MethodInfo methodInfo = typeof(SampleInheritCls).ResolveMethod("GetCount", new Type[] { typeof(string), typeof(int) });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[0].ParameterType);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[1].ParameterType);
        }

        /// <summary>
        /// 继承类重写方法，并且得到继承类的实现
        /// SampleInheritCls
        /// string GetCount(int i, string name)
        /// </summary>
        [TestMethod]
        public void ResolveMethodTwoParamterInheritSame_GetInherit()
        {
            MethodInfo methodInfo = typeof(SampleInheritCls).ResolveMethod("GetCount", new Type[] { typeof(int), typeof(string) });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[1].ParameterType);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[0].ParameterType);
            SampleInheritCls cls = new SampleInheritCls();
            string text = (string)methodInfo.Invoke(cls, new object[] { 1, string.Empty });
            Assert.AreEqual("SampleInheritCls", text);
        }

        /// <summary>
        /// SampleCls
        /// void GetCount<T>(int i) { }
        /// </summary>
        [TestMethod]
        public void GenericResolveMethodOneParamterArgument_One_String_Int()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new Type[] { typeof(string) }, MethodParameter.Create<int>());
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(1, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[0].ParameterType);
            Assert.AreEqual(1, methodInfo.GetGenericArguments().Length);
            Assert.AreEqual(typeof(string), methodInfo.GetGenericArguments()[0]);
        }

        /// <summary>
        /// SampleCls
        /// string GetCount<T>(T arg, string name)
        /// </summary>
        [TestMethod]
        public void GenericResolveMethodOneParamterArgument_One_Int_String()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new Type[] { typeof(int) }
                                                                                , new MethodParameter[] { MethodParameter.CreateGeneric<int>(), MethodParameter.Create<string>() });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[0].ParameterType);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[1].ParameterType);

            Assert.AreEqual(1, methodInfo.GetGenericArguments().Length);
            Assert.AreEqual(typeof(int), methodInfo.GetGenericArguments()[0]);
        }

        /// <summary>
        /// SampleInheritCls
        /// string GetCount<T>(T arg, string name)
        /// </summary>
        [TestMethod]
        public void GenericResolveMethodOneParamterArgumentInheritSame_One_Int_String()
        {
            MethodInfo methodInfo = typeof(SampleInheritCls).ResolveMethod("GetCount", new Type[] { typeof(int) }
                                                                                , new MethodParameter[] { MethodParameter.CreateGeneric<int>(), MethodParameter.Create<string>() });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[0].ParameterType);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[1].ParameterType);

            SampleInheritCls cls = new SampleInheritCls();
            string text = (string)methodInfo.Invoke(cls, new object[] { 1, string.Empty });
            Assert.AreEqual("SampleInheritCls", text);
        }



        /// <summary>
        /// SampleCls
        /// void GetCount(ref int i) { }
        /// </summary>
        [TestMethod]
        public void RefResolveMethodOneParamter_Int()
        {
            for (int i = 0; i < 3; i++)
            {
                MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new MethodParameter[]{
                                                                                        MethodParameter.Create<int>(true)
                                                                                    });
                Assert.IsNotNull(methodInfo);
                Assert.AreEqual("GetCount", methodInfo.Name);
                Assert.AreEqual(1, methodInfo.GetParameters().Length);
                Assert.AreEqual(typeof(int).MakeByRefType(), methodInfo.GetParameters()[0].ParameterType);
            }
        }

        /// <summary>
        /// SampleCls
        /// GetCount<T>(ref T name,int i) { }
        /// </summary>
        [TestMethod]
        public void RefResolveMethodOneParamter_string_Int()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", new Type[] { typeof(string) }, new MethodParameter[]{
                                                                                        MethodParameter.CreateGeneric<string>(true),
                                                                                        MethodParameter.Create<int>()
                                                                                    });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual(true, methodInfo.GetParameters()[0].ParameterType.IsByRef);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[0].ParameterType.GetElementType());

            Assert.AreEqual(typeof(int), methodInfo.GetParameters()[1].ParameterType);
        }


        /// <summary>
        /// SampleCls
        /// string GetCount<T>(T arg, string name)
        /// 获取泛型版本的方法
        /// </summary>
        [TestMethod]
        public void GenericResolveMethod_Generic()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", 1
                                                                                , new MethodParameter[] { MethodParameter.CreateGeneric(), MethodParameter.Create<string>() });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual("T", methodInfo.GetParameters()[0].ParameterType.Name);
            Assert.AreEqual(typeof(string), methodInfo.GetParameters()[1].ParameterType);

            Assert.AreEqual(1, methodInfo.GetGenericArguments().Length);
        }

        /// <summary>
        /// SampleCls
        /// void GetCount{T1,T2}(T1 name, T2 i) { }
        /// 获取泛型版本的方法
        /// </summary>
        [TestMethod]
        public void GenericResolveMethod_Generic_TwoParament()
        {
            MethodInfo methodInfo = typeof(SampleCls).ResolveMethod("GetCount", 2
                                                                                , new MethodParameter[] { MethodParameter.CreateGeneric(), MethodParameter.CreateGeneric() });
            Assert.IsNotNull(methodInfo);
            Assert.AreEqual("GetCount", methodInfo.Name);
            Assert.AreEqual(2, methodInfo.GetParameters().Length);
            Assert.AreEqual("T1", methodInfo.GetParameters()[0].ParameterType.Name);
            Assert.AreEqual("T2", methodInfo.GetParameters()[1].ParameterType.Name);

            Assert.AreEqual(2, methodInfo.GetGenericArguments().Length);
        }

        private class SampleCls
        {
            public void GetCount() { }
            public void GetCount(int i) { }
            public void GetCount(ref int i) { }
            public virtual string GetCount(int i, string name) { return "SampleCls"; }
            public void GetCount(string name, int i) { }
            public void GetCount<T>(int i) { }
            public void GetCount<T>(ref T name, int i) { }
            public virtual string GetCount<T>(T arg, string name) { return "SampleCls"; }

            public void GetCount<T1, T2>(T1 name, T2 i) { }

        }

        private class SampleInheritCls : SampleCls
        {
            public override string GetCount(int i, string name)
            {
                return "SampleInheritCls";
            }

            public override string GetCount<T>(T arg, string name)
            {
                return "SampleInheritCls";
            }

        }


    }
}
