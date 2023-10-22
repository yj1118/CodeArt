using System;
using System.Linq;

namespace CodeArt.Diagnostics
{
    /// <summary>
    /// 逻辑断言
    /// </summary>
    public static class LogicAssert
    {
        public static void IsTrue(bool expression)
        {
            if (!expression)
                throw new Exception("断言失败");
        }

        public static void AreEqual<T>(T expected, T actual)
        {
            if (!expected.Equals(actual))
                throw new Exception("断言失败");
        }

    }
}
