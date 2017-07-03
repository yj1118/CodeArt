using System;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeArt.Log;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;

namespace CodeArt.Util
{
    /// <summary>
    /// 专为 .net4.6以下版本提供的支持
    /// </summary>
    public static class EmptyArray<T>
    {
        public static readonly T[] Value;

        static EmptyArray()
        {
            EmptyArray<T>.Value = new T[0];
        }
    }
}
