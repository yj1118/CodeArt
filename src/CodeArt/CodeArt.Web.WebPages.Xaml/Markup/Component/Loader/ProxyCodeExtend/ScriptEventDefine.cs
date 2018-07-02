using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 脚本事件定义
    /// </summary>
    public class ScriptEventDefine 
    {
        /// <summary>
        /// 原始事件名
        /// </summary>
        public string OriginalName
        {
            get;
            private set;
        }

        /// <summary>
        /// 在客户端脚本中的事件名称
        /// </summary>
        public string ClientName
        {
            get;
            private set;
        }

        /// <summary>
        /// 通过定义的代码来初始化脚本事件的定义
        /// </summary>
        /// <param name="defineCode"></param>
        public ScriptEventDefine(string defineCode)
        {
            ArgumentAssert.IsNotNullOrEmpty(defineCode, "defineCode");

            var pos = defineCode.IndexOf(":");
            if (pos > -1)
            {
                this.OriginalName = defineCode.Substring(0, pos);
                this.ClientName = defineCode.Substring(pos + 1);
            }
            else
            {
                this.OriginalName = this.ClientName = defineCode;
            }
        }

    }
}
