using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Reflection;
using System.Configuration;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.IO;
using CodeArt.Runtime;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    internal sealed class XPCCoder
    {
        #region 路径

        public string VirtualPath
        {
            get;
            private set;
        }

        /// <summary>
        /// g.i文件
        /// </summary>
        public string GIPath
        {
            get;
            private set;
        }

        /// <summary>
        /// g文件
        /// </summary>
        public string GPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户代码文件
        /// </summary>
        public string UserPath
        {
            get;
            private set;
        }

        public string SourcePath
        {
            get;
            private set;
        }

        private static string GetFileName(string virtualPath, string suffix)
        {
            var path = virtualPath.TrimStart("/");
            int pos = path.LastIndexOf(".");
            if (pos > 0) path = path.Substring(0, pos);
            path = path.Replace("/", "\\");
            path = string.Format("{0}.{1}cs", path, suffix);

            return !string.IsNullOrEmpty(suffix) ? Path.Combine(App_CodePath, "g", path) : Path.Combine(App_CodePath, path);
        }

        public static string App_CodePath
        {
            get
            {
                var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
                return Path.Combine(baseDirectory, "App_Code");
            }
        }


        //public static string App_CodePath
        //{
        //    get
        //    {
        //        var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        //        return Path.Combine(baseDirectory, App_CodeAssemblyName);
        //    }
        //}

        //public static string App_CodeAssemblyName
        //{
        //    get
        //    {
        //        var app_code = ConfigurationManager.AppSettings["app_code"];
        //        if (string.IsNullOrEmpty(app_code)) throw new XamlException("没有配置app_code");
        //        return app_code;
        //    }
        //}


        private static string GetSourceFileName(string virtualPath)
        {
            var path = virtualPath.TrimStart("/");
            path = path.Replace("/", "\\");
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        #endregion

        public static XPCCoder Create(string virtualPath)
        {
            var code = PageUtil.GetRawCode(virtualPath);
            if (!XamlUtil.IsDeclareXaml(code)) return null;
            return new XPCCoder(virtualPath);
        }

        private XPCCoder(string virtualPath)
        {
            this.VirtualPath = virtualPath;
            this.SourcePath = GetSourceFileName(this.VirtualPath);
            this.GIPath = GetFileName(this.VirtualPath, "g.i.");
            this.GPath = GetFileName(this.VirtualPath, "g.");
            this.UserPath = GetFileName(this.VirtualPath, null);
            this.PageCode = PageUtil.GetRawCode(virtualPath);
            InitCode();
        }

        #region 代码

        public string PageCode
        {
            get;
            private set;
        }

        public string GICode
        {
            get;
            private set;
        }

        public string GCode
        {
            get;
            private set;
        }

        public string UserCode
        {
            get;
            private set;
        }

        private void InitCode()
        {
            this.GICode = string.Empty;
            this.GCode = string.Empty;
            this.UserCode = string.Empty;

            if (string.IsNullOrEmpty(this.PageCode)) return;
            var node = XamlUtil.GetNode(this.PageCode);
            if (node == null) this.PageCode = string.Empty; //没有节点，证明不是xaml文件
            var baseComponentType = node.MapComponentType();
            if (baseComponentType == null) return;
            var className = GetClassName(this.VirtualPath);

            var connectedObjects = new List<ConnectedObject>();
            ParseConnectedObjects(node, connectedObjects);

            this.GICode = GetCode_g_i(connectedObjects, baseComponentType, className, this.VirtualPath);
            this.GCode = GetCode_g(connectedObjects, baseComponentType, className);
            this.UserCode = GetCode_user(baseComponentType, className);
        }

        #endregion

        /// <summary>
        /// 生成xamlPage组件代码并保存
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns>如果代表有更改，那么返回true，否则false</returns>
        public void Generate()
        {
            if (!File.Exists(this.SourcePath))
            {
                Delete();
            }
            else
            {
                Save_user();
                Save(this.GCode, this.GPath);
                Save(this.GICode, this.GIPath);
            }
        }

        private sealed class ConnectedObject
        {
            public string Name { get; private set; }

            public Type Type { get; private set; }

            public ConnectedObject(string name, Type type)
            {
                this.Name = name;
                this.Type = type;
            }
        }

        private static void ParseConnectedObjects(HtmlNode node, IList<ConnectedObject> objs)
        {
            var name = node.GetDeclareName();
            if (!string.IsNullOrEmpty(name))
            {
                objs.Add(new ConnectedObject(name, node.MapComponentType()));
            }
            foreach (var child in node.ChildNodes)
            {
                ParseConnectedObjects(child, objs);
            }
        }

        #region g.i 文件

        private static string GetCode_g_i(IList<ConnectedObject> objs, Type baseComponentType,string className,string virtaulPath)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("using CodeArt.Web.WebPages;");
            code.AppendLine("using CodeArt.Web.WebPages.Xaml.Markup;");
            code.AppendFormat("using {0};", baseComponentType.Namespace);
            code.AppendLine();
            code.AppendLine();
            code.AppendLine("namespace App_Code");
            code.AppendLine("{");
            code.AppendFormat("    public partial class {0} : {1}", className,baseComponentType.Name);
            code.AppendLine();
            code.AppendLine("    {");
            foreach(var obj in objs)
            {
                code.AppendFormat("        private {0} {1};", obj.Type.FullName, obj.Name);
                code.AppendLine();
            }
            code.AppendLine();
            code.AppendFormat("        const string _virtualPath = \"{0}\";", virtaulPath);
            code.AppendLine();
            code.AppendLine("        private bool _contentLoaded;");
            code.AppendLine();
            code.AppendLine("        public void InitializeComponent()");
            code.AppendLine("        {");
            code.AppendLine("            if (_contentLoaded)");
            code.AppendLine("                return;");
            code.AppendLine();
            code.AppendLine("            _contentLoaded = true;");
            code.AppendLine();
            code.AppendLine("            var xaml = PageUtil.GetRawCode(_virtualPath);");
            code.AppendLine("            XamlReader.LoadComponent(this, xaml);");
            code.AppendLine("        }");
            code.AppendLine("    }");
            code.Append("}");
            return code.ToString();
        }

        #endregion

        #region g 文件

        private static string GetCode_g(IList<ConnectedObject> objs, Type baseComponentType, string className)
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("using CodeArt.Web.WebPages;");
            code.AppendLine("using CodeArt.Web.WebPages.Xaml.Markup;");
            code.AppendFormat("using {0};", baseComponentType.Namespace);
            code.AppendLine();
            code.AppendLine();
            code.AppendLine("namespace App_Code");
            code.AppendLine("{");
            code.AppendFormat("    public partial class {0} : {1}, IComponentConnector", className, baseComponentType.Name);
            code.AppendLine();
            code.AppendLine("    {");
            code.AppendLine();
            code.AppendLine("        public void Connect(string connectionName, object target)");
            code.AppendLine("        {");
            code.AppendLine("            switch (connectionName)");
            code.AppendLine("            {");

            foreach (var obj in objs)
            {
                code.AppendFormat("                case \"{0}\":", obj.Name);
                code.AppendLine();
                code.AppendLine("                    {");
                code.AppendFormat("                        this.{0} = ({1})(target);", obj.Name, obj.Type.FullName);
                code.AppendLine();
                code.AppendLine("                    }");
                code.AppendLine("                    break;");
            }
            code.AppendLine("            }");
            code.AppendLine("        }");
            code.AppendLine();
            code.AppendLine("        public CodeArt.Web.WebPages.Xaml.DependencyObject Find(string connectionName)");
            code.AppendLine("        {");
            code.AppendLine("            switch (connectionName)");
            code.AppendLine("            {");

            foreach (var obj in objs)
            {
                code.AppendFormat("                case \"{0}\":", obj.Name);
                code.AppendLine();
                code.AppendLine("                    {");
                code.AppendFormat("                        return this.{0};", obj.Name, obj.Type.FullName);
                code.AppendLine();
                code.AppendLine("                    }");
            }
            code.AppendLine("            }");
            code.AppendLine("            return null;");
            code.AppendLine("        }");
            code.AppendLine("    }");
            code.Append("}");
            return code.ToString();
        }

        #endregion


        #region 用户文件

        private void Save_user()
        {
            var fileName = this.UserPath;
            if (File.Exists(fileName)) return; //已存在用户文件，不用重写
            IOUtil.CreateFileDirectory(fileName);
            File.WriteAllText(fileName, this.UserCode, Encoding.UTF8);
        }


        private string GetCode_user(Type baseComponentType, string className)
        {
            var fileName = this.UserPath;
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName, Encoding.UTF8);
            }
            else
            {
                StringBuilder code = new StringBuilder();
                code.AppendLine("using CodeArt.Web.WebPages;");
                code.AppendLine("using CodeArt.Web.WebPages.Xaml.Markup;");
                code.AppendFormat("using {0};", baseComponentType.Namespace);
                code.AppendLine();
                code.AppendLine();
                code.AppendLine("namespace App_Code");
                code.AppendLine("{");
                code.AppendFormat("    public partial class {0} : {1}", className, baseComponentType.Name);
                code.AppendLine();
                code.AppendLine("    {");
                code.AppendFormat("        public {0}()",className);
                code.AppendLine();
                code.AppendLine("        {");
                code.AppendLine("            this.InitializeComponent();");
                code.AppendLine("        }");
                code.AppendLine("    }");
                code.Append("}");
                return code.ToString();
            }
        }

        #endregion


        #region 存储

        public static string GetClassName(string virtualPath)
        {
            var path = virtualPath.TrimStart("/");
            int pos = path.LastIndexOf(".");
            if (pos > 0) path = path.Substring(0, pos);
            return path.Replace("/", "_");
        }

        /// <summary>
        /// 删除生成的文件
        /// </summary>
        /// <param name="virtualPath"></param>
        public void Delete()
        {
            var fileName = this.GIPath;
            if (File.Exists(fileName)) File.Delete(fileName);

            fileName = this.GPath;
            if (File.Exists(fileName)) File.Delete(fileName);
        }

        /// <summary>
        /// 返回true标示文件发生更改了，更新文件成功
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool Save(string code, string fileName)
        {
            if (string.IsNullOrEmpty(code)) return false; //不保存空代码

            if (File.Exists(fileName))
            {
                var originalCode = File.ReadAllText(fileName, Encoding.UTF8);
                if (code.Equals(originalCode, StringComparison.Ordinal)) return false; //代码相同，不必保存
            }
            IOUtil.CreateFileDirectory(fileName);
            File.WriteAllText(fileName, code, Encoding.UTF8);
            return true;
        }



        #endregion

    }


}
