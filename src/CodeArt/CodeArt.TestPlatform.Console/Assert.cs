using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.TestPlatform
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if(!IsEqual(expected,actual))
            {
                throw new AssertException(string.Format("期望{0},实际{1}", expected, actual));
            }
        }

        public static void AreEqual(object expected, object actual)
        {
            AreEqual(expected, actual);
        }


        public static void AreNotEqual<T>(T expected, T actual)
        {
            if (IsEqual(expected, actual))
            {
                throw new AssertException(string.Format("期望{0},实际{1}", expected, actual));
            }
        }

        public static void AreNotEqual(object expected, object actual)
        {
            AreNotEqual(expected, actual);
        }


        private static bool IsEqual<T>(T expected, T actual)
        {
            return object.Equals(expected, actual);
        }

        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                throw new AssertException("期望true,实际false");
            }
        }

        public static void IsFalse(bool condition)
        {
            if (!condition)
            {
                throw new AssertException("期望false,实际true");
            }
        }
    }

    public class AssertException : UserUIException
    {
        public AssertException(string message)
            : base(message)
        {

        }
    }

}
