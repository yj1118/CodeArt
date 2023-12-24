using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    public interface IScriptView
    {
        /// <summary>
        /// 获取脚本视图对应的数据型的代码
        /// </summary>
        /// <returns></returns>
        string GetDataCode();

        /// <summary>
        /// 获取脚本视图对应的可执行的代码
        /// </summary>
        /// <returns></returns>
        string GetScriptCode();
    }
}
