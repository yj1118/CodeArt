using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.TestPlatform
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestCaseAttribute : Attribute
    {
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        public TestCaseAttribute()
        {
        }
    }
}

