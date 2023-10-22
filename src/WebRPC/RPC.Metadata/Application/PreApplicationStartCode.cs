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
using RPC.Common;

[assembly: PreApplicationStart(typeof(RPC.Metadata.PreApplicationStartCode), "Start")]

namespace RPC.Metadata
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
