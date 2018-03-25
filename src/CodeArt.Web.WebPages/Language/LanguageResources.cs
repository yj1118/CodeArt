using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Xml;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages
{
    public static class LanguageResources
    {
        /// <summary>
        /// 获取当前语言选项下的字符串
        /// </summary>
        /// <param name="name">字符串名称</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            return GetStringsResource(Language.Current.Name,name);
        }

        /// <summary>
        /// 获取指定语言选项下的字符串
        /// </summary>
        /// <param name="language"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Get(string language,string name)
        {
            return GetStringsResource(language, name);
        }

        /// <summary>
        /// 获取资源文件Strings下的字符串数据，Strings作为保留的资源文件关键字专门用于存储多语言的字符串数据
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        private static string GetStringsResource(string language, string resourceKey)
        {
            var resources = Resources.Get(language);
            string value = string.Empty;
            resources.TryGetValue(resourceKey, out value);
            return value;
        }


        private static LazyIndexer<string, Dictionary<string,string>> Resources = new LazyIndexer<string, Dictionary<string, string>>((language)=>
        {
            var resoucesKey = "Strings";
            if (!string.IsNullOrEmpty(language)) resoucesKey = string.Format("Strings.{0}", language);
            var fileName = Path.Combine(WebUtil.GetAbsolutePath(), "Resources", string.Format("{0}.resx", resoucesKey));
            if (!File.Exists(fileName))
            {
                if (string.IsNullOrEmpty(language)) return new Dictionary<string, string>();
                return Resources.Get(string.Empty); //如果没有找到语言文件，那么使用默认的语言
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var nodes = doc.SelectNodes("root/data");
            Dictionary<string, string> data = new Dictionary<string, string>(nodes.Count);
            foreach(XmlNode node in nodes)
            {
                var name = node.GetAttributeValue("name");
                var value = node.SelectSingleNode("value").InnerXml;
                data.Add(name, value);
            }
            return data;
        });


        internal static void OnChange(string virtualPath)
        {
            Resources.Clear();
        }


    }
}
