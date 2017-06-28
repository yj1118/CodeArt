using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CodeArt.Util
{
    public static class CharExtensions
    {
        public static bool IgnoreCaseEquals(this char c, char value)
        {
            return c.ToLower() == value.ToLower();
        }

        public static char ToUpper(this char c)
        {
            return Char.ToUpper(c);
        }

        public static char ToLower(this char c)
        {
            return Char.ToLower(c);
        }

        public static bool IsLetter(this char c)
        {
            return Char.IsLetter(c);
        }

        public static bool IsDigit(this char c)
        {
            return Char.IsDigit(c);
        }


    }
}
