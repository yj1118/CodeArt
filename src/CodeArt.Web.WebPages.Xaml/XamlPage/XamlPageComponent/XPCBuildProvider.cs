using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Web;

using Microsoft.CSharp;
using CodeArt.Web.WebPages;
using CodeArt.IO;

namespace CodeArt.Web.WebPages.Xaml
{
    public class XPCBuildProvider : BuildProvider
    {
        public override void GenerateCode(AssemblyBuilder assemblyBuilder)
        {
            Generate(this.VirtualPath);
        }

        internal static void Generate(string virtualPath)
        {
            XPCCoder coder = XPCCoder.Create(virtualPath);
            if (coder == null) return;
            coder.Generate();//生成代码
            if (!IsIgnore(coder))
            {
                if (WriteCode(coder) || !XPCAssembly.Exists(coder))
                {
                    try
                    {
                        //重新生成程序集
                        XPCAssembly.Compile(coder);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }


        private static bool IsIgnore(XPCCoder coder)
        {
            return !File.Exists(coder.UserPath) || !File.Exists(coder.GPath) || !File.Exists(coder.GIPath);
        }

        /// <summary>
        /// 保存代码文件，如果成功保存，返回true，这意味着代码被更改
        /// </summary>
        /// <param name="coder"></param>
        /// <returns></returns>
        private static bool WriteCode(XPCCoder coder)
        {
            int count = 0;
            if (WriteCode(coder.UserPath, coder.UserCode)) count++;
            if (WriteCode(coder.GPath, coder.GCode)) count++;
            if (WriteCode(coder.GIPath, coder.GICode)) count++;
            return count > 0;
        }

        /// <summary>
        /// 保存代码文件，如果成功保存，返回true，这意味着代码被更改
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool WriteCode(string fileName,string code)
        {
            var targetFileName = MapCacheFile(fileName);
            IOUtil.CreateFileDirectory(targetFileName);
            if (!File.Exists(targetFileName))
            {
                File.WriteAllText(targetFileName, code, Encoding.UTF8);
                return true;
            }
            var targetCode = File.ReadAllText(targetFileName);
            if (targetCode.Equals(code, StringComparison.Ordinal)) return false; //对比新老代码是否相同
            File.WriteAllText(targetFileName, code, Encoding.UTF8);
            return true;
        }

        private static string MapCacheFile(string fileName)
        {
            var seg = fileName.Replace(XPCCoder.App_CodePath, string.Empty).Trim('\\');
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "$XamlPage", "App_Code");
            return Path.Combine(path, seg);
        }

        //private static void DeleteCode(string fileName)
        //{
        //    var targetFileName = MapCacheFile(fileName);
        //    IOUtil.CreateFileDirectory(targetFileName);
        //    File.Delete(targetFileName);
        //}

    }


}
