using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.Util;
using System.Diagnostics;
using CodeArt.Log;
using CodeArt.Concurrent;
using CodeArt.Runtime;

namespace CodeArtTest.Util
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class HashCoderTest
    {
        [TestMethod]
        public void Common()
        {
            var obj = new ClassOne();
            var code = obj.GetHashCode();

            obj = new ClassOne();
            var code2 = obj.GetHashCode();

            Assert.AreEqual(code, code2);

            obj = new ClassOne("哈哈");
            var code3 = obj.GetHashCode();

            var obj1 = new ClassOne("嘿嘿");
            var code4 = obj1.GetHashCode();

            Assert.AreNotEqual(code3, code4);

            var obj2 = new ClassOne("嘿嘿");
            var code5 = obj2.GetHashCode();

            Assert.AreEqual(code4, code5);

            Assert.IsTrue(obj1.Equals(obj2));
            Assert.IsFalse(obj.Equals(obj2));

        }



        [TestMethod]
        public void Complex()
        {
            MethodParameter p = MethodParameter.Create<int>();
            var code = p.GetHashCode();

            MethodKey mk = new MethodKey(typeof(AppContext), "test", null, null, false);
            var mkCode = mk.GetHashCode();
        }

        [TestMethod]
        public void Speed()
        {
            const int max = 1000000;
            var obj0 = new ClassOne("哈哈");
            Stopwatch s = new Stopwatch();
            s.Restart();
            for(var i=0;i<max;i++)
            {
                var code = obj0.GetHashCode();
            }
            var elapsed0 = s.ElapsedMilliseconds;

            var obj1 = new ClassOneCommon("哈哈");
            s.Restart();
            for (var i = 0; i < max; i++)
            {
                var code = obj1.GetHashCode();
            }
            var elapsed1 = s.ElapsedMilliseconds;
        }


        private class ClassOne
        {
            public string Name
            {
                get;
                private set;
            }

            public string Name2
            {
                get;
                private set;
            }

            public string Name3
            {
                get;
                private set;
            }

            public ClassOne One
            {
                get;
                private set;
            }


            public ClassOne()
            {
                this.Name = string.Empty;
                this.Name2 = string.Empty;
                this.Name3 = string.Empty;
            }

            public ClassOne(string name)
            {
                this.Name = name;
                this.Name2 = name;
                this.Name3 = name;
                this.One = new ClassOne();

            }


            public override bool Equals(object obj)
            {
                return this.GetHashCode() == obj.GetHashCode();
            }

            public override int GetHashCode()
            {
                return HashCoder<ClassOne>.Combine(HashCoder<string>.GetCode(this.Name),
                   HashCoder<string>.GetCode(this.Name2),
                   HashCoder<string>.GetCode(this.Name3),
                   HashCoder<ClassOne>.GetCode(this.One));

            }
        }

        private class ClassOneCommon
        {
            public string Name
            {
                get;
                private set;
            }


            public string Name2
            {
                get;
                private set;
            }

            public string Name3
            {
                get;
                private set;
            }

            public ClassOneCommon One
            {
                get;
                private set;
            }

            public ClassOneCommon()
            {
                this.Name = string.Empty;
                this.Name2 = string.Empty;
                this.Name3 = string.Empty;
            }

            public ClassOneCommon(string name)
            {
                this.Name = name;
                this.Name2 = name;
                this.Name3 = name;
                this.One = new ClassOneCommon();

            }

            public override int GetHashCode()
            {
                return HashCoder<ClassOneCommon>.Combine(HashCoder<string>.GetCode(this.Name),
                                   HashCoder<string>.GetCode(this.Name2),
                                   HashCoder<string>.GetCode(this.Name3),
                                   HashCoder<ClassOneCommon>.GetCode(this.One));
            }

        }



        internal struct MethodKey
        {
            public Type ObjectType
            {
                get;
                private set;
            }


            public string MethodName
            {
                get;
                private set;
            }

            public Type[] GenericTypes
            {
                get;
                private set;
            }


            public MethodParameter[] Parameters
            {
                get;
                private set;
            }

            /// <summary>
            /// 是否为泛型版本（即没有定义实际类型的泛型方法，例如 Add{T}(T obj)
            /// </summary>
            public bool IsGenericVersion
            {
                get;
                private set;
            }

            public MethodKey(Type objectType, string methodName, Type[] genericTypes, MethodParameter[] parameters, bool isGenericVersion)
            {
                this.ObjectType = objectType;
                this.MethodName = methodName;
                this.GenericTypes = genericTypes;
                this.Parameters = parameters;
                this.IsGenericVersion = isGenericVersion;
            }

            public override int GetHashCode()
            {
                return HashCoder<MethodKey>.Combine(HashCoder<Type>.GetCode(this.ObjectType)
                                                    , HashCoder<string>.GetCode(this.MethodName)
                                                    , HashCoder<Type>.GetCode(this.GenericTypes)
                                                    , HashCoder<MethodParameter>.GetCode(this.Parameters)
                                                    , HashCoder<bool>.GetCode(this.IsGenericVersion));
            }

            public override bool Equals(object obj)
            {
                return obj.GetHashCode() == this.GetHashCode();
            }

            /// <summary>
            /// 仅根据名称匹配
            /// </summary>
            //public bool IsOnlyMathName
            //{
            //    get
            //    {
            //        return this.GenericTypes.Length == 0 && this.Parameters.Length == 0;
            //    }
            //}
        }

    }
}
