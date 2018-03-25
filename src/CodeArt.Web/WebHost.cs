using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.Web
{
    public struct WebHost
    {
        public string Protocol
        {
            get;
            private set;
        }

        public string Domain
        {
            get;
            private set;
        }

        public string Prefix
        {
            get;
            private set;
        }

        /// <summary>
        /// 域名后缀
        /// </summary>
        public string Suffix
        {
            get;
            private set;
        }

        public string Host
        {
            get;
            private set;
        }

        public string Url
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">例如 http://www.codeart.cn/index.htm </param>
        public WebHost(string url)
        {
            this.Protocol = GetProtocol(url);
            this.Host = GetHost(url);
            var info = Parse(this.Host);
            this.Prefix = info.Prefix;
            this.Domain = info.Domain;
            this.Suffix = info.Suffix;
            this.Url = string.Format("{0}://{1}/", this.Protocol, this.Host);
        }

        private static string GetProtocol(string url)
        {
            int pos = url.IndexOf("://");
            if (pos == -1) return "http";
            return url.Substring(0, pos);
        }

        private static string GetHost(string url)
        {
            int pos = url.IndexOf("://");
            if (pos > -1) url = url.Remove(0, pos + 3);

            string host = null;
            pos = url.IndexOf("/");
            if (pos < 0) host = url;
            else host = url.Substring(0, pos);

            pos = host.IndexOf(":");
            return pos < 0 ? host : host.Substring(0, pos);
        }

        private static (string Prefix,string Domain,string Suffix) Parse(string host)
        {
            string prefix = null;
            string domain = null;
            string sx = null;
            foreach (string suffix in DomainSuffixs)
            {
                if (host.EndsWith(suffix))
                {
                    int pos = host.LastIndexOf(suffix);
                    string seg = host.Substring(0, pos);
                    int lastDotPos = seg.LastIndexOf('.');
                    if (lastDotPos != -1)
                    {
                        prefix = seg.Substring(0, lastDotPos);
                        seg = seg.Substring(lastDotPos + 1);
                    }
                    else prefix = string.Empty;

                    domain = string.Format("{0}{1}", seg, suffix);
                    sx = suffix.Substring(1);
                }
            }

            if (prefix == null)
                throw new WebException(string.Format(Strings.UnableParseHost, host));

            return (prefix, domain, sx);
        }


        public static readonly string[] DomainSuffixs = new string[] { ".com",".cn",".mobi",".co",".net",".com.cn",".net.cn",".so",".org",".gov.cn",".org.cn",".tel"
                                                                                        ,".tv",".biz",".cc",".hk",".name",".info",".asia",".me",".help" };
    }
}
