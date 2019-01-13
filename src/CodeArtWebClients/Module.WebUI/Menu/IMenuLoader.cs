using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Linq;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.IO;
using CodeArt;
using CodeArt.Web;

using System.Threading;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using System.Web;

namespace Module.WebUI
{
    public interface IMenuHandler
    {
        /// <summary>
        /// 处理菜单项
        /// </summary>
        /// <param name="menuItem"></param>
        void Process(DTObject menuItem);

        /// <summary>
        /// 处理根目录
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <returns></returns>
        string GetFolder(string rootFolder);
    }
}
