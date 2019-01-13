using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Linq;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt;
using CodeArt.IO;

using System.Threading;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace Module.WebUI
{
    /// <summary>
    /// 
    /// </summary>
    internal static class MenuLocal
    {
        private static string _version = "0";

        static MenuLocal()
        {
            _version = XmlMenuLoader.LoadVersion();
        }


        private static Func<string, Func<string, DTObject>> _getLocalMenu = LazyIndexer.Init<string, Func<string, DTObject>>((language) =>
        {
            return (markedCode) =>
            {
                var dto = XmlMenuLoader.Load(language, markedCode);
                if (dto.IsEmpty()) return DTObject.Empty;
                return dto.AsReadOnly();
            };
        });

        /// <summary>
        /// 获取本地的菜单信息
        /// </summary>
        /// <returns></returns>
        public static string GetMenuCode(string language, string markedCode)
        {
            if(Principal.IsLogin)
            {
                var userId = Principal.Id.ToString();
                var fileName = MenuHelper.GetMenuFileName(_version, language, markedCode, userId);
                if (!File.Exists(fileName))
                {
                    var menu = _getLocalMenu(language)(markedCode);
                    var roles = Principal.Roles;
                    var real = FilterMenu(menu, roles);
                    string code = real == null ? "{childs:[]}" : real.GetCode();
                    CodeArt.IO.IOUtil.CreateFileDirectory(fileName);
                    File.WriteAllText(fileName, code);
                    return code;
                }
                return File.ReadAllText(fileName); //从缓存读取
            }
            else
            {
                var fileName = MenuHelper.GetMenuFileName(_version, language, markedCode, "$public");
                if (!File.Exists(fileName))
                {
                    var menu = _getLocalMenu(language)(markedCode);
                    string code = menu == null ? "{childs:[]}" : menu.GetCode();
                    CodeArt.IO.IOUtil.CreateFileDirectory(fileName);
                    File.WriteAllText(fileName, code);
                    return code;
                }
                return File.ReadAllText(fileName); //从缓存读取
            }
        }

        internal static void RemoveMenuCode(string userId)
        {
            var folder = MenuHelper.GetMenuFolder(_version, userId);
            IOUtil.Delete(folder);
        }

        /// <summary>
        /// 根据登录人拥有的角色过滤菜单项
        /// </summary>
        /// <param name="rawMenu"></param>
        /// <param name="principalRoles"></param>
        /// <returns></returns>
        private static DTObject FilterMenu(DTObject rawMenu, Principal.Role[] principalRoles)
        {
            var menuRoles = rawMenu.GetList("roles", false);
            if (menuRoles != null)
            {
                var roleCodes = menuRoles.ToArray<string>();
                var result = principalRoles.FirstOrDefault((role) =>
                {
                    return roleCodes.Contains(role.MarkedCode, StringComparer.OrdinalIgnoreCase);
                });
                if (result == null) return null; //当前登录人没有菜单需要的角色，不能显示
            }

            DTObject menu = DTObject.Create();
            foreach (var member in _outputMembers)
            {
                if (rawMenu.Exist(member))
                    menu[member] = rawMenu[member];
            }

            rawMenu.Each("childs", (child) =>
            {
                var target = FilterMenu(child, principalRoles);
                if (target != null) menu.Push("childs", target);
            });

            return menu;
        }

        private static readonly string[] _outputMembers = new string[] { "name", "icon", "iconFontSize", "tags", "code" };


    }
}
