using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    public class SqlIn
    {
        /// <summary>
        /// 占位符，用于生成sql语句时替换用
        /// </summary>
        public string Placeholder
        {
            get;
            private set;
        }

        /// <summary>
        /// 匹配的参数名称
        /// </summary>
        public string ParamName
        {
            get;
            private set;
        }

        internal SqlIn(string paramName, string placeholder)
        {
            this.ParamName = paramName;
            this.Placeholder = placeholder;
        }
    }
}