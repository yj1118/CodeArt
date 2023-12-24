using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.IO;

using CodeArt.AppSetting;

[assembly: ProApplicationStarted(typeof(CodeArt.WeChat.ProApplicationStarted), "Initialize")]

namespace CodeArt.WeChat
{
    internal class ProApplicationStarted
    {
        public static void Initialize()
        {
            WeChatToken.Initialize();
        }
    }
}
