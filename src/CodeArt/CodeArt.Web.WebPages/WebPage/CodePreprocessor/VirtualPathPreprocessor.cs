using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Text.RegularExpressions;

using CodeArt.Util;
using CodeArt.IO;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 基于虚拟路径的代码预处理器
    /// </summary>
    [SafeAccess]
    public class VirtualPathPreprocessor : ICodePreprocessor
    {
        protected VirtualPathPreprocessor() {
            _getCode = LazyIndexer.Init<string, string>(LoadCode, (value) => { return value != null; });
            _getSetting = LazyIndexer.Init<string, WebPageSetting>(LoadSetting);
        }

        #region 获取原始代码

        private readonly Func<string, string> _getCode = null;

        public string ReadCode(string path)
        {
#if (DEBUG)
            return LoadCode(path);
#endif

#if (!DEBUG)
            return _getCode(path);
#endif
        }


        private string LoadCode(string virtualPath)
        {
            string rawCode = LoadRawCode(virtualPath);
            if (rawCode == null) return null;
            //移除命令行
            Regex reg = new Regex(@"<%@.*?%>\s*");
            MatchCollection mcs = reg.Matches(rawCode);
            if (mcs.Count == 0) return rawCode;
            return reg.Replace(rawCode, string.Empty);
        }


        /// <summary>
        /// 获取原始代码
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        protected virtual string LoadRawCode(string virtualPath)
        {
            var path = virtualPath.TrimStart('/').Replace("/", "\\");
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (!File.Exists(fileName)) return null;
            string code = null;
            using (StreamReader stream = new StreamReader(fileName, Encoding.UTF8))
            {
                code = stream.ReadToEnd();
            }
            return code;
        }


        #endregion

        #region 读取页面配置


        private Func<string, WebPageSetting> _getSetting = null;

        public WebPageSetting ReadSetting(string path)
        {
#if (DEBUG)
            return LoadSetting(path);
#endif

#if (!DEBUG)
            return _getSetting(path);
#endif
        }

        /// <summary>
        /// 读取文本流
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        private StreamReader GetTextReader(string path)
        {
            string rawCode = LoadRawCode(path);
            if (rawCode == null) return null;

            byte[] codeBytes = rawCode.GetBytes(Encoding.UTF8);
            MemoryStream ms = new MemoryStream(codeBytes);
            return new StreamReader(ms);
        }

        private WebPageSetting LoadSetting(string path)
        {
            WebPageSetting setting = new WebPageSetting();
            using (StreamReader reader = GetTextReader(path))
            {
                if (reader == null) return null;
                Regex reg = new Regex("<%@ *(\\w+) +(.*?)%>");
                Regex parReg = null;
                while (true)
                {
                    string lineCode = reader.ReadLine();
                    if (lineCode == null) break;
                    Match m = reg.Match(lineCode);
                    if (!m.Success) break;
                    if (parReg == null)
                        parReg = new Regex(" *([^=]+) *= *\"([^=]+)\"");
                    string name = m.Groups[1].Value;
                    string parCode = m.Groups[2].Value;

                    NameValueCollection args = new NameValueCollection();
                    MatchCollection mcs = parReg.Matches(parCode);

                    foreach (Match pm in mcs)
                    {
                        args.Add(pm.Groups[1].Value, pm.Groups[2].Value);
                    }
                    setting.AddItem(name, args);
                }
            }
            return setting;
        }

        

        #endregion

        public bool IsValidPath(string path)
        {
            string code = ReadCode(path);
            return code != null;
        }

        public static readonly ICodePreprocessor Instance = new VirtualPathPreprocessor();
    }
}
