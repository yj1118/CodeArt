using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using CodeArt.DTO;

namespace CodeArt.CI
{
    public static class Util
    {
        /// <summary>
        /// 获得<paramref name="fileName"/>所在目录的片段信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetFolderSegments(string fileName)
        {
            string[] temp = fileName.Split('\\');
            return temp.Take(temp.Length - 1);
        }

        public static DTObject LoadConfig(string appConfig)
        {
            var document = new XmlDocument();
            document.LoadXml(File.ReadAllText(appConfig));
            var ci = document.SelectSingleNode("configuration/appSettings/add[@key='ci']");
            if (ci == null) return DTObject.Empty;
            var value = ci.Attributes["value"].Value;
            return DTObject.Create(value) ;
        }
    }
}