using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    internal class AssetGenerator
    {
        private AssetGenerator() { }

        public GenerateResult Generate(string resourcePath, AssetMapper mapper)
        {
            string[] temp = resourcePath.Split(',');
            string typeName = temp[0];
            string assemblyName = temp[1];
            var token = GetToken(assemblyName);  //由于组件的短名称可能重复，所以需要完整的名称（程序集.短名称），所以我们需要对程序集编码，在url路径中不暴露程序集的名称

            var localTypeName = typeName.Substring(assemblyName.Length + 1);
            var assetName = mapper.GetAssetName(localTypeName);

            StringBuilder vp = new StringBuilder(localTypeName.Substring(0, localTypeName.Length - assetName.Length));
            vp = vp.Replace(assemblyName, string.Empty);
            vp = vp.Replace(".", "/");
            vp.Append(assetName);
            var localVirtualPath = string.Format("/{0}", vp.ToString());
            localVirtualPath = mapper.MapPath(localVirtualPath);
            var virtualPath = string.Format("/assets/{0}{1}", token, localVirtualPath);
            var physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath.Replace('/', '\\').Substring(1));

#if DEBUG
            WriteBytes(physicalPath, resourcePath);
#endif

#if !DEBUG
                    if (!File.Exists(physicalPath))
                    {
                        WriteBytes(physicalPath, resourcePath);
                    }
#endif
            return new GenerateResult(virtualPath, localVirtualPath);
        }

        private static void WriteBytes(string physicalPath,string assetPath)
        {
            IOUtil.CreateFileDirectory(physicalPath);
            var bytes = AssemblyResource.LoadBytes(assetPath);

            try
            {
                File.WriteAllBytes(physicalPath, bytes);
            }
            catch(Exception ex)
            {
                //写日志即可，有时候同一时间访问一个文件，有可能冲突，但是在发布模式下不会遇到该问题，所以只用写日志
                Log.LogWrapper.Default.Fatal(ex);
            }
            
        }


        internal static string GetToken(string assemblyName)
        {
            return assemblyName.ToBase64(Encoding.UTF8);
        }

        public static AssetGenerator Instance = new AssetGenerator();

        internal class GenerateResult
        {
            /// <summary>
            /// 由于组件的短名称可能重复，所以需要完整的名称（程序集.短名称），所以我们需要对程序集编码，在url路径中不暴露程序集的名称
            /// </summary>
            public string VirtualPath { get; private set; }

            /// <summary>
            /// 由于组件的短名称可能重复，所以需要完整的名称（程序集.短名称），所以我们需要对程序集编码，在url路径中不暴露程序集的名称
            /// </summary>
            public string LocalVirtualPath { get; private set; }

            public GenerateResult(string virtualPath, string localVirtualPath)
            {
                this.VirtualPath = virtualPath;
                this.LocalVirtualPath = localVirtualPath;
            }
        }


    }
}
