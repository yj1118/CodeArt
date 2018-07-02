using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Linq;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt;
using System.Threading;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.IO;

namespace Module.WebUI
{
    /// <summary>
    /// 还需重构，新版中没测试
    /// </summary>
    internal static class MenuRemote
    {
        private static string GetServerCode(string userId, string markedCode, Func<IEnumerable<UIMenuItem>> getServerCode)
        {
            ArgumentAssert.IsNotNullOrEmpty(markedCode, "markedCode");

            string fileName = GetMenuFileName(userId, markedCode);
            if (File.Exists(fileName)) return File.ReadAllText(fileName);

            UIMenu menu = new UIMenu(getServerCode());
            var code = menu.ToJSON();

            try
            {
                WriteText(fileName, code);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return code;
        }

        #region 服务器端定义的菜单



        private static string GetMenuFileName(string userId, string markedCode)
        {
            string folder = MenuHelper.GetMenuFolder(GetVersion(),userId);
            return string.IsNullOrEmpty(markedCode)
                    ? folder
                    : Path.Combine(folder, markedCode.ToBase64(Encoding.UTF8));
        }



        internal static void RemoveMenuCode(string userId)
        {
            var folder = MenuHelper.GetMenuFolder(GetVersion(), userId);
            IOUtil.Delete(folder);
        }

        #region 版本控制


        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// 更新菜单版本号,该方法适用于全局更新
        /// </summary>
        public static string UpdateVersion()
        {
            string version = GetVersion();
            string newVersion = (long.Parse(version) + 1).ToString();
            string fileName = Path.Combine(MenuHelper.GetRootFolder(), "version");
            _lock.Write(() =>
            {
                WriteText(fileName, newVersion);
            });
            return newVersion;
        }

        /// <summary>
        /// 获取菜单版本号
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            string fileName = Path.Combine(MenuHelper.GetRootFolder(), "version");
            if (!File.Exists(fileName)) return "0";
            string version = string.Empty;
            _lock.Read(() =>
            {
                version = File.ReadAllText(fileName);
            });
            return version;
        }

        #endregion

        private static void WriteText(string fileName, string content)
        {
            FileInfo file = new FileInfo(fileName);
            if (!file.Directory.Exists) Directory.CreateDirectory(file.Directory.FullName);

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(content);
                }
            }
        }

        #endregion

        #region 获取菜单代码


        /// <summary>
        /// 
        /// </summary>
        /// <param name="markedCode">菜单识别码</param>
        /// <param name="isLocal">是否为本地菜单</param>
        /// <returns></returns>
        public static string GetMenuCode(string language, string markedCode)
        {
            //从服务器端获取用户菜单
            var userId = Principal.Id.ToString();

            var code = GetServerCode(userId.ToString(), markedCode, () =>
            {
                var data = ServiceContext.Invoke("getMenuCode", (arg) =>
                {
                    arg["userId"] = userId;
                    arg["markedCode"] = markedCode;
                });

                return MapMenuItems(data);
            });

            return code;
        }

        private static IEnumerable<UIMenuItem> MapMenuItems(DTObject data)
        {
            List<UIMenuItem> items = new List<UIMenuItem>();
            if (data.GetValue<int>("dataCount") == 0) return items;

            foreach (var itemData in data.GetList("rows"))
            {
                FillItems(itemData, items);
            }
            return items;
        }

        private static List<UIMenuItem> MapChildItems(DTObjects data)
        {
            List<UIMenuItem> items = new List<UIMenuItem>();

            foreach (var itemData in data)
            {
                FillItems(itemData, items);
            }
            return items;
        }

        private static void FillItems(DTObject itemData, List<UIMenuItem> items)
        {
            var name = itemData.GetValue<string>("name", string.Empty);
            var icon = itemData.GetValue<string>("icon", string.Empty);
            var iconFontSize = itemData.GetValue<string>("iconFontSize", string.Empty);
            var tags = string.Join(",", itemData.GetList("tags").ToArray<string>());
            var code = itemData.GetValue<string>("code", string.Empty);

            UIMenuItem item = new UIMenuItem(name, icon, iconFontSize, tags, code);
            var childs = itemData.GetList("childs", false);
            if (childs != null)
            {
                item.AddChilds(MapChildItems(childs));
            }

            items.Add(item);
        }

        #endregion
    }
}
