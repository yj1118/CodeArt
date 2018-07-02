using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.AOP;
using CodeArt.Common;

using CodeArt.Web.WebPages;
using CodeArt.Web;

namespace Module.WebUI
{
    /// <summary>
    /// 每一个插件页面都需要打上该标签
    /// </summary>
    [Singleton()]
    public class AutoLoginAspect : AspectInitializerBase
    {
        /// <summary>
        /// 是否必须登录
        /// </summary>
        public bool Must
        {
            get;
            private set;
        }

        public AutoLoginAspect(bool must)
        {
            this.Must = must;
        }

        public override void Init()
        {
            if (!Principal.IsLogin())
            {
                var context = WebPageContext.Current;
                var principalId = context.GetQueryValue<string>("principalId", string.Empty);
                var principalName = context.GetQueryValue<string>("principalName", string.Empty);

                if (string.IsNullOrEmpty(principalId) || string.IsNullOrEmpty(principalName))
                {
                    if (!this.Must) return;
                    throw new WebException("没有提供负责人信息");
                }
                Principal.Login(principalId, principalName);
            }
        }
    }
}


