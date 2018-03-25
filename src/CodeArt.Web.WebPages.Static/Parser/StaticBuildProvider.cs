using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Web;

using Microsoft.CSharp;
using CodeArt.IO;

namespace CodeArt.Web.WebPages.Static
{
    public class StaticBuildProvider : BuildProvider
    {
        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            if (BuildProviderUtil.RequiredBuild())
            {
                GenerateCode(this.VirtualPath, true);
            }
        }

        public static bool IsIgnore(string virtualPath)
        {
            return virtualPath.Contains("$Static");
        }

        private static string GetCodeFileName(string virtualPath)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"$Static");
            string path = virtualPath.Replace("/", "\\");
            if (path[0] == '\\') path = path.Substring(1);
            var fileName = Path.Combine(folder, path);
            //return fileName.Length > 230 ? fileName.BalancedPath(Path.Combine(folder,"$Long")) : fileName;
            return fileName;
        }

        public static void GenerateCode(string virtualPath, bool isRemoveCache)
        {
            if (!IsIgnore(virtualPath))
            {
                string code = PageUtil.GetRawCode(virtualPath);
                var info = StaticParser.Parse(virtualPath, code);
                if (BuildProviderUtil.SaveCode(GetCodeFileName(virtualPath), info)
                    && isRemoveCache)
                {
                    RemoveCache();
                }
            }
        }


        public static string GetDynamicCode(string virtualPath)
        {
            string code = string.Empty;
            if (!_cache.TryGetValue(virtualPath, out code))
            {
                lock (_syncObject)
                {
                    if (!_cache.TryGetValue(virtualPath, out code))
                    {
                        code = GetCode(virtualPath);
                        _cache.Add(virtualPath, code);
                    }
                }
            }
            return code;
        }

        private static object _syncObject = new object();
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();

        private static string GetCode(string virtualPath)
        {
            GenerateCode(virtualPath, false);
            string fileName = GetCodeFileName(virtualPath);
            if(File.Exists(fileName))
                return BuildProviderUtil.ReadCode(fileName);

            //不参与解析的代码,直接输出原始代码
            return PageUtil.GetRawCode(virtualPath);
        }

        public static void RemoveCache()
        {
            //由于静态资源可能被嵌套插入，所以得全部清理
            _cache.Clear();
        }
    }


}
