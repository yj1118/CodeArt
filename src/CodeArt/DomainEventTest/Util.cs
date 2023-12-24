using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DomainEventTest
{
    public static class Util
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            if (!Object.Equals(expected, actual))
                WriteLine(string.Format("期望：{0}，实际：{1}", expected, actual), ConsoleColor.Red);
        }

        public static void AreEqual(object expected, object actual)
        {
            if (!Object.Equals(expected, actual))
                WriteLine(string.Format("期望：{0}，实际：{1}", expected, actual), ConsoleColor.Red);
        }


        public static void Error(string msg)
        {
            WriteLine(string.Format("错误:{0}", msg), ConsoleColor.Red);
        }

        public static void WriteLine(string value, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
