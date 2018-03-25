using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    public class SqlAny
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

        /// <summary>
        /// 有效代码
        /// </summary>
        public string Content
        {
            get;
            private set;
        }

        internal SqlAny(string paramName, string placeholder,string content)
        {
            this.ParamName = paramName;
            this.Placeholder = placeholder;
            this.Content = content;
        }
    }
}