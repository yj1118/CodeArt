using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web;
using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Util;
using CodeArt.Web.WebPages;
using CodeArt.ServiceModel;

namespace Module.WebUI.Xaml
{
    public static class VirtualFileUtil
    {
        /// <summary>
        /// 获取当前登录人对应的磁盘空间的根目录信息
        /// </summary>
        /// <returns></returns>
        public static dynamic GetRootDirectory()
        {
            var id = Principal.Id;
            var result = (dynamic)ServiceContext.Invoke("GetRootDirectory", (arg) =>
            {
                arg["UserId"] = id;
            });
            return result;
        }
    }
}
   