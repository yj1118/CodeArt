using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 资产包,资产包里的"文件夹"名称只能为英文、数字、下划线，如果使用"-"，.NET会自动转成为下划线
    /// 所以请预先处理资产包的文件夹名称
    /// 文件名称不受此限制
    /// </summary>
    public class AssetPackage
    {
        public string Path
        {
            get;
            set;
        }

        //public string Key
        //{
        //    get;
        //    set;
        //}

        public AssetFile[] Files
        {
            get;
            private set;
        }

        public AssetFile GetFile(string path)
        {
            if (this.Files == null) return null;
            foreach (var item in this.Files)
            {
                if (item.Path == path) return item;
            }
            return null;
        }

        internal void Generate(string assemblyName, AssetMapper mapper)
        {
            var resuorcePath = mapper.MapPath(this.Path);
            if (!resuorcePath.EndsWith("/")) resuorcePath += "/";  //该代码主要是为了路径匹配时使用，避免文件名和路径名匹配导致的BUG

            var resourceNames = _getResourceNames(assemblyName);

            List<AssetFile> files = new List<AssetFile>();

            foreach (var resourceName in resourceNames)
            {
                //资源程序集形式的路径
                var assemblyPath = resourceName.Substring(assemblyName.Length + 1);
                var assetName = mapper.GetAssetName(assemblyPath);

                string path = "/";
                if (assetName == assemblyPath)
                    path += assetName;
                else
                {
                    var temp = assemblyPath.Substring(0,assemblyPath.Length - assetName.Length);
                    path += temp.Replace('.', '/') + assetName;
                }

                if (path.StartsWith(resuorcePath))
                {
                    AssetFile file = new AssetFile();
                    file.Path = path;
                    file.Generate(assemblyName, mapper);
                    files.Add(file);
                }
            }

            this.Files = files.ToArray();
        }
        


        private static Func<string, string[]> _getResourceNames = LazyIndexer.Init<string, string[]>((assemblyName) =>
        {
            var assembly = Assembly.Load(assemblyName);
            var resourceNames = assembly.GetManifestResourceNames();
            return resourceNames;
        });
    }
}
