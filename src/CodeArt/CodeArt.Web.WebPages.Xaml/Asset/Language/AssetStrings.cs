using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Runtime;
using CodeArt.IO;


namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 该对象用于处理js中包含的文本的多语言输出问题，多语言版本中，不仅html页的内容要呈现对应语言的文本，JS的文本内容也需要
    /// </summary>
    public class AssetStrings : CodeAsset
    {
        public bool IsGlobal
        {
            get;
            private set;
        }

        public string VirtualPath
        {
            get;
            private set;
        }

        private string GetGlobalCode(string language)
        {
            if(string.IsNullOrEmpty(language)) return string.Format("<script type=\"text/javascript\" src=\"/assets/strings/global.js?v={0}\" charset=\"utf-8\"></script>", Application.Current.AssetVersion);
            return string.Format("<script type=\"text/javascript\" src=\"/assets/strings/global.{0}.js?v={1}\" charset=\"utf-8\"></script>", language, Application.Current.AssetVersion);
        }

        private string GetPageCode(string language)
        {
            var path = WebPageContext.Current.VirtualPath;
            var pos = path.LastIndexOf(".");
            path = path.Substring(0, pos).TrimStart("/");

            if (string.IsNullOrEmpty(language)) return string.Format("<script type=\"text/javascript\" src=\"/assets/strings/page/{0}.js?v={1}\" charset=\"utf-8\"></script>", path, Application.Current.AssetVersion);
            return string.Format("<script type=\"text/javascript\" src=\"/assets/strings/page/{0}.{1}.js?v={2}\" charset=\"utf-8\"></script>", path, language, Application.Current.AssetVersion);
        }

        private Func<string, string> _getCode;
        private Func<string, bool> _generateFiles;

        protected override string GetCode()
        {
            return _getCode(Language.Current.Name);
        }

        public override DrawOrigin Origin {
            get => DrawOrigin.Header;
            set
            {
                throw new NotImplementedException();
            }
        }

        private AssetStrings(string virtualPath)
        {
            this.IsGlobal = string.IsNullOrEmpty(virtualPath);
            _getCode = this.IsGlobal ? LazyIndexer.Init<string, string>(GetGlobalCode) : LazyIndexer.Init<string, string>(GetPageCode);
            _generateFiles = LazyIndexer.Init<string, bool>(GenerateFiles);
        }

        public static readonly AssetStrings Global = new AssetStrings(null);

        /// <summary>
        /// 当前页面的字符串资产
        /// </summary>
        public static AssetStrings Current
        {
            get
            {
                return _getStrings(WebPageContext.Current.VirtualPath);
            }
        }

        private static Func<string, AssetStrings> _getStrings = LazyIndexer.Init<string, AssetStrings>((virtualPath) =>
        {
            return new AssetStrings(virtualPath);
        });

        protected override void Draw(PageBrush brush)
        {
            _generateFiles(Language.Current.Name);
            if (this.Origin == DrawOrigin.Header || this.Origin == DrawOrigin.Bottom)
            {
                brush.Backspace();
            }
            brush.DrawLine(GetCode(), this.Origin);
        }

        #region  生成语言文件

        
        private bool GenerateFiles(string language)
        {
            var code = BuildFileCode(language);
            var fileName = GetFileName(language);
            IOUtil.CreateFileDirectory(fileName);
            System.IO.File.WriteAllText(fileName, code, Encoding.UTF8);
            return true;
        }

        private string BuildFileCode(string language)
        {
            StringBuilder code = new StringBuilder();
            if (this.IsGlobal)
            {
                code.AppendFormat("var $$language=\"{0}\";", language);
                code.Append("var $$strings = {};");
            }
            if (_data.Count() > 0)
            {
                code.AppendLine("(function () {");
                code.AppendLine("var s=$$strings;");
                foreach (var p in _data)
                {
                    var assemblyName = p.Key;
                    var keys = p.Value;
                    var assembly = AssemblyUtil.Get(assemblyName);
                    if (assembly == null)
                    {
                        foreach (var resourceKey in keys)
                        {
                            var value = LanguageResources.Get(language, resourceKey) ?? string.Empty;
                            code.AppendFormat("s[\"{0}\"]={1};", resourceKey, JSON.GetCode(value));
                            code.AppendLine();
                        }
                    }
                    else
                    {
                        var manager = new ResourceManager(string.Format("{0}.Strings", assembly.GetName().Name), assembly);
                        var ci = LanguageUtil.GetCulture(language); ;
                        foreach (var resourceKey in keys)
                        {
                            var value = manager.GetString(resourceKey, ci) ?? string.Empty;
                            code.AppendFormat("s[\"{0}\"]={1};", resourceKey, JSON.GetCode(value));
                            code.AppendLine();
                        }
                    }
                }
                code.Append("})();");
            }
            return code.ToString();
        }

        private string GetFileName(string language)
        {
            if (this.IsGlobal)
            {
                var path = string.IsNullOrEmpty(language) ? "/assets/strings/global.js" : string.Format("/assets/strings/global.{0}.js", language);
                return WebUtil.MapAbsolutePath(path);
            }
            else
            {
                var path = WebPageContext.Current.VirtualPath;
                var pos = path.LastIndexOf(".");
                path = path.Substring(0, pos).TrimStart("/");

                path = string.IsNullOrEmpty(language) ? string.Format("/assets/strings/page/{0}.js", path) 
                                                      : string.Format("/assets/strings/page/{0}.{1}.js", path, language);
                return WebUtil.MapAbsolutePath(path);
            }
        }


        private MultiDictionary<string, string> _data = new MultiDictionary<string, string>(false);

        internal void Append(string assemblyName,string[] keys)
        {
            lock(_data)
            {
                _data.TryAdd(assemblyName, keys);
            }
        }

        /// <summary>
        /// 追加站点资源文件下的字符串
        /// </summary>
        /// <param name="keys"></param>
        public void Append(params string[] keys)
        {
            Append("x", keys); //追加站点资源文件下的字符串
        }

        #endregion

    }
}
