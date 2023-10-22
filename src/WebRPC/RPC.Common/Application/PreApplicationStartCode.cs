using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.IO;

using CodeArt.Web.WebPages;
using CodeArt.AppSetting;
using CodeArt.Security;
using CodeArt.ServiceModel;

[assembly: PreApplicationStart(typeof(RPC.Common.PreApplicationStartCode), "Start")]

namespace RPC.Common
{
    public class PreApplicationStartCode
    {
        public static void Start()
        {
            AuthFilterFactory.Register(AuthFilter.Instance);
            //支持灰度功能的过滤
            AppDark.RegisterFilter(DarkFilter.Instance);
        }
    }
}
