using System;
using System.Collections.Generic;

namespace CodeArt.Util
{
    public static class Algorithm
    {
        public static T Balance<T>(IList<T> values)
        {
            if (values.Count == 0) return default(T);
            if (values.Count == 1) return values[0];
            var index = new Random().Next(0, values.Count);
            return values[index];
        }
     }
}
