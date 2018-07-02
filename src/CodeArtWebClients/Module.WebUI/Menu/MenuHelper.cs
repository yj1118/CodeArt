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
    /// <summary>
    /// 由于云化，所以缓存机制应该单独设计成模块，由数据库存储todo...
    /// </summary>
    public static class MenuHelper
    {
        internal static string GetMenuFileName(string version,string language, string markedCode, string userId)
        {
            string folder = GetMenuFolder(version, userId);
            return Path.Combine(folder, language, markedCode);
        }

        /// <summary>
        /// 得到账号对应的菜单文件所在的目录
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        internal static string GetMenuFolder(string version, string userId)
        {
            string name = userId.MD5();
            string folder0 = name.Substring(0, 8), folder1 = name.Substring(9, 8);
            return Path.Combine(GetRootFolder(), folder0, folder1, version, name);
        }

        internal static string GetRootFolder()
        {
            var folder = ConfigurationManager.AppSettings["menu-cache-folder"];
            if (folder == null)
            {
                folder = "../.menu"; //默认在根目录
                folder = CodeArt.AppContext.MapPath(folder);
            }
            return folder;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="markedCode">菜单识别码</param>
        /// <param name="isLocal">是否为本地菜单</param>
        /// <returns></returns>
        public static string GetMenuCode(string language, string markedCode, bool isLocal)
        {
            return isLocal ? MenuLocal.GetMenuCode(language, markedCode) : MenuRemote.GetMenuCode(language, markedCode);
        }

        /// <summary>
        /// 删除某个用户的菜单文件
        /// </summary>
        /// <param name="language"></param>
        /// <param name="markedCode"></param>
        /// <param name="isLocal"></param>
        /// <returns></returns>
        public static void RemoveMenuCode(string userId)
        {
            MenuLocal.RemoveMenuCode(userId);
            MenuRemote.RemoveMenuCode(userId);
        }

        public static void RemoveMenuCode(IEnumerable<string> userIds)
        {
            foreach (var userId in userIds)
            {
                MenuLocal.RemoveMenuCode(userId);
                MenuRemote.RemoveMenuCode(userId);
            }
        }

        /// <summary>
        /// 删除所有菜单信息
        /// </summary>
        public static void RemoveAllMenuCode()
        {
            IOUtil.ClearDirectory(GetRootFolder());
        }

    }
}
