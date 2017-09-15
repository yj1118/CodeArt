using System;
using System.Linq;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 堆栈断言,该断言不是用于测试的，而是用于实际运行的判断
    /// </summary>
    internal static class StackAssert
    {
        private static string GetExceptionMessage(StackItem actual, StackItem expected, string typeComparePhrase)
        {
            return string.Format(
                "在计算堆栈上发现意外的项 ItemType=\"{0}\" 并且类型 Type=\"{1}\".预期的项是 ItemType=\"{2}\" 并且类型{3} Type=\"{4}\".",
                actual.ItemType,
                actual.Type.Name,
                expected.ItemType,
                typeComparePhrase,
                expected.Type.Name);
        }

        public static void AreEqual(StackItem actual, Type expected)
        {
            AreEqual(actual, new StackItem(expected));
        }

        public static void AreEqual(StackItem actual, StackItem expected)
        {
            const string phrase = "等于";

            if (expected.ItemType != actual.ItemType)
            {
                throw new StackValidationException(GetExceptionMessage(actual, expected, phrase));
            }

            if (expected.Type != actual.Type)
            {
                throw new StackValidationException(GetExceptionMessage(actual, expected, phrase));
            }
        }

        /// <summary>
        /// 判断actual为基元类型
        /// <para>???????????此处有问题：</para>
        /// <para>需要测试string变量的判断</para>
        /// <para>string虽然不是值类型，但是是基元类型</para>
        /// <para>分析代码中的算法,string被认为不是基元类型，所以需要测试</para>
        /// </summary>
        /// <param name="actual"></param>
        public static void IsPrimitive(StackItem actual)
        {
            var type = actual.Type.IsEnum ? Enum.GetUnderlyingType(actual.Type) : actual.Type;
            //值类型不一定是基元类型
            if (actual.ItemType != ItemType.Value && !type.IsPrimitive)
                throw new StackValidationException("期望计算堆栈上的数据为基元类型，但是得到的是 " + actual);
        }

        /// <summary>
        /// 确定actual的数据类型，是否与expected类型匹配
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="permitNull"></param>
        public static void IsAssignable(StackItem actual, Type expected, bool permitNull)
        {
            IsAssignable(actual, new StackItem(expected), permitNull);
        }

        /// <summary>
        /// <para>确定 expected 是否可以从 actual 分配</para>
        /// <para>即：确定actual的数据类型，是否与expected的数据类型匹配(比如是actual.Type是expected.Type的子类、实现了expected.Type类型的接口等)</para>
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="permitNull">是否允许为null</param>
        public static void IsAssignable(StackItem actual, StackItem expected, bool permitNull)
        {
            const string phrase = "可转换为";

            if (permitNull
                && expected.ItemType == ItemType.Reference //期望的对象为引用类型，才有可能为null
                && actual.Type.IsPrimitive //如果实际对象不是基类型，那么在代码段1之后的逻辑中判断，如果是基类型，
                && GetPrimitiveType(actual.Type) == PrimitiveType.I4)//那么判断它在栈中占有4个字节才有可能为null，因为null占用堆栈大小为4个字节;
            {
                //允许引用类型为null
                return;
            }

            //代码段1：
            if (expected.ItemType != actual.ItemType)
            {
                throw new StackValidationException(GetExceptionMessage(actual, expected, phrase));
            }

            if (expected.Type.IsPrimitive && actual.Type.IsPrimitive)
            {
                if (!ArePrimitivesCompatible(actual.Type, expected.Type))
                {
                    throw new StackValidationException(GetExceptionMessage(actual, expected, phrase));
                }
            }
            else if (expected.Type.IsEnum && actual.Type.IsPrimitive)
            {
                if (!ArePrimitivesCompatible(Enum.GetUnderlyingType(expected.Type), actual.Type))
                {
                    throw new StackValidationException(GetExceptionMessage(actual, expected, phrase));
                }
            }
            else
            {
                //类型不是基元类型，那么通过Type.IsAssignableFrom判断
                //IsAssignableFrom可以得知,
                //1.actual.Type与expected.Type相等
                //或
                //2.actual.Type是expected.Type的子类
                //或
                //3.actual.Type实现了expected.Type的接口...
                if (!expected.Type.IsAssignableFrom(actual.Type))
                {
                    throw new StackValidationException(GetExceptionMessage(actual, expected, phrase));
                }
            }
        }

        /// <summary>
        /// 基元类型
        /// </summary>
        private enum PrimitiveType
        {
            /// <summary>
            /// 32字节
            /// </summary>
            I4,
            /// <summary>
            /// 64字节
            /// </summary>
            I8,
            /// <summary>
            /// 浮点数4字节
            /// </summary>
            R4,
            /// <summary>
            /// 浮点数8字节
            /// </summary>
            R8
        }

        private static PrimitiveType GetPrimitiveType(Type type)
        {
            if (type == typeof(bool)) return PrimitiveType.I4;
            if (type == typeof(char)) return PrimitiveType.I4;
            if (type == typeof(byte)) return PrimitiveType.I4;
            if (type == typeof(short)) return PrimitiveType.I4;
            if (type == typeof(int)) return PrimitiveType.I4;
            if (type == typeof(sbyte)) return PrimitiveType.I4;
            if (type == typeof(ushort)) return PrimitiveType.I4;
            if (type == typeof(uint)) return PrimitiveType.I4;
            if (type == typeof(decimal)) return PrimitiveType.I4;

            if (type == typeof(long)) return PrimitiveType.I8;
            if (type == typeof(ulong)) return PrimitiveType.I8;

            if (type == typeof(float)) return PrimitiveType.R4;

            if (type == typeof(double)) return PrimitiveType.R8;

            throw new ArgumentException("type不是基元类型", "type");
        }

        /// <summary>
        /// <para>基元类型是否兼容</para>
        /// <para>在32位系统中，堆栈每个数据单元的大小为4字节。</para>
        /// <para>小于等于4字节的数据，比如字节、字、双字和布尔型，在堆栈中都是占4个字节的；
        /// 大于4字节的数据在堆栈中占4字节整数倍的空间。</para>
        /// <para>对于bool、char、byte、short、int、sbyte、ushort、uint、decimal类型，他们是可以互相兼容的（都占4字节）</para>
        /// <para>对于long、ulong类型，他们是可以互相兼容的（都占8字节）</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool ArePrimitivesCompatible(Type left, Type right)
        {
            return GetPrimitiveType(left) == GetPrimitiveType(right);
        }
    }
}
