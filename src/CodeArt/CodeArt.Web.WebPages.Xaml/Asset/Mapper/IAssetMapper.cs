using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 资产映射器主要是为了解决html资产的名称和.net资源名称冲突的问题
    /// 在.net中，会将目录名称的'-'符号转换为'_'
    /// </summary>
    internal interface IAssetMapper
    {
        /// <summary>
        /// 修正路径片段，将资产路径片段转为资源路径片段,也可以将资源路径片段转换为资产路径片段
        /// </summary>
        /// <param name="pathSegment"></param>
        /// <returns></returns>
        string Correct(string pathSegment);

        /// <summary>
        /// 得到资产的文件名
        /// </summary>
        /// <param name="resuorceName"></param>
        /// <returns></returns>
        string GetAssetName(string resuorceName);


    }
}
