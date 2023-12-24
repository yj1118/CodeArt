using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.IO;

using CodeArt.AppSetting;

[assembly: PreApplicationEnd(typeof(CodeArt.WeChat.PreApplicationEnd), "Dispose")]

namespace CodeArt.WeChat
{
    internal class PreApplicationEnd
    {
        public static void Dispose()
        {
            WeChatToken.Dispose();
        }
    }
}
