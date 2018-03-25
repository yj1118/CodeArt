using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class AssetString
    {
        public string Keys
        {
            get;
            set;
        }

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="assemblyName"></param>
        internal void Generate(string assemblyName)
        {
            var keys = this.Keys.Split(',').Select((t) => t.Trim()).ToArray();
            AssetStrings.Current.Append(assemblyName, keys);
        }
    }
}
