using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Configuration;

using CodeArt.DTO;
using CodeArt.Web;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace Module.WebUI
{
    public static class DomainUtil
    {
        public static string GetUrl(string domainKey, string path)
        {
            var domain = GetDomain(domainKey);
            return WebUtil.Combine(domain, path);
        }

        public static string GetDomain(string domainKey, bool throwError = true)
        {
            var domain = ConfigurationManager.AppSettings[string.Format("domain-{0}", domainKey)];
            if (string.IsNullOrEmpty(domain) && throwError) throw new WebException("没有找到" + domainKey + "的域名配置");
            return domain;
        }
    }
}