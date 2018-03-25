using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [DebuggerDisplay("{Path}")]
    public class AssetFile
    {
        /// <summary>
        /// 文件的本地访问路径，不是程序集形式的路径
        /// </summary>
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

        /// <summary>
        /// 文件可以访问的虚拟路径（网络访问路径，该路径包括程序集编码等信息）
        /// </summary>
        public string VirtualPath
        {
            get;
            private set;
        }

        ///// <summary>
        ///// 本地虚拟路径
        ///// </summary>
        //public string LocalVirtualPath
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="assemblyName"></param>
        internal void Generate(string assemblyName, AssetMapper mapper)
        {
            //文件的程序集路径
            var assemblyPath = this.Path.TrimStart('/').Replace('/','.');
            var resourcePath = string.Format("{0}.{1},{0}", assemblyName, assemblyPath);
            var result = AssetGenerator.Instance.Generate(resourcePath, mapper);
            this.VirtualPath = result.VirtualPath;
            //this.LocalVirtualPath = result.LocalVirtualPath.Substring(parentPath.Length);
            //if (string.IsNullOrEmpty(this.Key)) //如果没有设置文件的键值，那么键值就为本地虚拟路径的值
            //    this.Key = this.LocalVirtualPath;
        }
    }
}
