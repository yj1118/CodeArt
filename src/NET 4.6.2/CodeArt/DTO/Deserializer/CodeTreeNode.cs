using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.DTO
{
    internal struct CodeTreeNode
    {
        public StringSegment Name
        {
            get;
            private set;
        }

        public StringSegment Value
        {
            get;
            private set;
        }

        public CodeType Type
        {
            get;
            private set;
        }

        public CodeTreeNode[] Childs
        {
            get;
            private set;
        }

        public CodeTreeNode(StringSegment name, StringSegment value, CodeType type, CodeTreeNode[] childs)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
            this.Childs = childs;
        }

        public CodeTreeNode(StringSegment name, StringSegment value, CodeType type)
            : this(name, value, type, Array.Empty<CodeTreeNode>())
        {
        }


        #region 静态成员

        public static bool IsList(StringSegment code)
        {
            if (code.StartsWith("["))
            {
                if (!code.EndsWith("]"))
                    throw new CodeFormatErrorException(code.ToString() + "以 [ 作为首字符，但是没有以 ] 结尾！ ");
                return true;
            }
            return false;
        }

        public static bool IsObject(StringSegment code)
        {
            if (code.StartsWith("{"))
            {
                if (!code.EndsWith("}"))
                    throw new CodeFormatErrorException(code.ToString() + "以 { 作为首字符，但是没有以 } 结尾！ ");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取JSON格式的值
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static CodeType GetValueType(StringSegment code)
        {
            var codeType = CodeType.NonStringValue;
            if (code.Length >= 2)
            {
                if ((code.StartsWith('\"') && code.EndsWith('\"'))
                    || (code.StartsWith('\'') && code.EndsWith('\'')))
                {
                    codeType = CodeType.StringValue;
                }
            }
            return codeType;
        }

        #endregion

    }
}