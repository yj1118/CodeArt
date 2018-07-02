using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Web;

using Microsoft.CSharp;
using CodeArt.Util;
using CodeArt.IO;

namespace CodeArt.Web.WebPages.Static
{
    public static class BuildProviderUtil
    {
        /// <summary>
        /// 静态资源只有在release模式下才需要生成时构建
        /// </summary>
        /// <returns></returns>
        public static bool RequiredBuild()
        {
#if(DEBUG)
            return false;
#endif
#if(!DEBUG)
            //release模式才需要在生成时构建
            return true;
#endif
        }

        #region 保存代码

        /// <summary>
        /// 保存代码
        /// </summary>
        /// <param name="info"></param>
        /// <param name="code"></param>
        /// <returns>如果代码已改变，则返回true,否则返回false</returns>
        public static bool SaveCode(string fileName, (string Code,bool Parsed) info)
        {
            if(info.Parsed)
            {
                var code = info.Code;
                var folder = Directory.GetParent(fileName);
                if (!folder.Exists) Directory.CreateDirectory(folder.FullName);

                if (!File.Exists(fileName))
                {
                    WriteCode(fileName, code);
                    return true;
                }
                else
                {
                    var oldCode = ReadCode(fileName);
                    if (!code.Equals(oldCode, StringComparison.Ordinal))
                    {
                        WriteCode(fileName, code);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if(File.Exists(fileName))
                {
                    //非解析代码不用保存，如果以前保存过，那证明之前是需要解析的代码，现在需要删除
                    File.Delete(fileName);
                    return true;
                }
                return false;
            }

            
        }

        public static string ReadCode(string fileName)
        {
            string code = string.Empty;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                {
                    code = reader.ReadToEnd();
                }
            }
            return code;
        }

        private static void WriteCode(string fileName, string code)
        {
            using(FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.Write(code);
                }
            }

            IOUtil.SetEveryoneFullControl(fileName);
        }

        #endregion
    }


}
