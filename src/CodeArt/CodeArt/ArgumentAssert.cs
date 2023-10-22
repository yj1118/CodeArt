using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeArt
{
    /// <summary>
    /// 封装了一些验证方法参数的方法
    /// </summary>
    public static class ArgumentAssert
    {
        /// <summary>
        /// 当value是null时，抛出ArgumentNullException
        /// 当value是empty时，抛出ArgumentException
        /// </summary>
        /// <param name="value">参数的值</param>
        /// <param name="parameterName">参数名称</param>
        public static void IsNotNullOrEmpty(string value, string parameterName)
        {
            if (value == null) throw new ArgumentNullException(parameterName);
            if (value.Length == 0) throw new ArgumentException(parameterName + " 不能为empty", parameterName);
        }

        public static void IsNotNullOrEmpty(IList value, string parameterName)
        {
            if (value == null) throw new ArgumentNullException(parameterName);
            if (value.Count == 0) throw new ArgumentException(parameterName + " 不能为empty", parameterName);
        }

        public static void IsNotNullOrEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            if (value == null) throw new ArgumentNullException(parameterName);
            if (value.Count() == 0) throw new ArgumentException(parameterName + " 不能为empty", parameterName);
        }


        /// <summary>
        /// 当value是null时候，抛出ArgumentNullException
        /// </summary>
        /// <param name="value">参数的值</param>
        /// <param name="parameterName">参数名称</param>
        public static void IsNotNull(object value, string parameterName)
        {
            if (value == null) throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// 当value为null时，抛出ArgumentNullException
        /// </summary>
        /// <typeparam name="T">参数的类型</typeparam>
        /// <param name="value">参数的值</param>
        /// <param name="parameterName">参数名称</param>
        public static void IsNotNull<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue) throw new ArgumentNullException(parameterName);
        }
    }
}
