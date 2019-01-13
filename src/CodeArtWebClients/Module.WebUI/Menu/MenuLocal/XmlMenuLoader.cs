using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Configuration;

using AppContext = CodeArt.AppContext;
using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 基于xml配置文件的菜单加载器
    /// </summary>
    public static class XmlMenuLoader
    {
        private static DTObject Load(string language, string fileName, string group)
        {
            DTObject dto = DTObject.Create("{childs:[{name,icon,code,childs:[{}]}]}");
            try
            {
                var groupNode = GetGroupNode(fileName,group);
                if (groupNode != null)
                {
                    foreach (XmlNode node in groupNode.ChildNodes)
                    {
                        if (node.NodeType != XmlNodeType.Element) continue;
                        var child = dto.CreateAndPush("childs");
                        FillMenuItem(child, node, language);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dto;
        }


        private static XmlNode GetGroupNode(string fileName,string groupName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            return doc.SelectSingleNode(string.Format("menus/group[@name='{0}']",groupName));
        }

        private static string GetNodeValue(XmlNode node,string  attrName)
        {
            return node.GetAttributeValue(attrName);
        }

        private static void FillMenuItem(DTObject item, XmlNode node, string language)
        {
            string url = GetNodeValue(node, "url"), target = GetNodeValue(node, "target");
            var codeDTO = DTObject.Create();
            if (!string.IsNullOrEmpty(url))
            {
                codeDTO.SetValue("url", url);
                if (!string.IsNullOrEmpty(target))
                {
                    codeDTO.SetValue("target", target);
                }
            }

            string name = GetNodeValue(node, "name");
            name = ParseName(name, language);
            string icon = GetNodeValue(node, "icon");
            string iconFontSize = GetNodeValue(node, "iconFontSize");
            string tags = GetNodeValue(node, "tags");
            string roles = GetNodeValue(node, "roles");

            if (!string.IsNullOrEmpty(icon)) item.SetValue("icon", icon);
            if (!string.IsNullOrEmpty(iconFontSize)) item.SetValue("iconFontSize", iconFontSize);
            if (!string.IsNullOrEmpty(tags))
            {
                var temp = tags.Split(',');
                item.Push("tags", temp, (t, tag) =>
                {
                    t.SetValue(tag);
                });
            }
            if (!string.IsNullOrEmpty(roles))
            {
                var temp = roles.Split(',');
                item.Push("roles", temp, (t, role) =>
                {
                    t.SetValue(role);
                });
            }
            item.SetValue("name", name);
            item.SetObject("code", codeDTO);

            var handler = MenuHelper.GetHandler();
            if(handler != null)
            {
                handler.Process(item);
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element) continue;
                var child = item.CreateAndPush("childs");
                FillMenuItem(child, childNode, language);
            }
        }

        private static string ParseName(string name,string language)
        {
            const string prefix = "{Strings.";
            const string suffix = "}";
            if (!(name.StartsWith(prefix) && name.EndsWith(suffix))) return name;
            name = name.Substring(prefix.Length, name.Length - prefix.Length - suffix.Length);
            return LanguageResources.Get(language, name);
        }

        public static DTObject Load(string language, string group)
        {
            string fileName = GetFileName();
            return Load(language, fileName, group);
        }

        public static string LoadVersion()
        {
            string fileName = GetFileName();
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var node = doc.SelectSingleNode("menus");
            return node.GetAttributeValue("version", "1");
        }

        private static string GetFileName()
        {
            string fileName = ConfigurationManager.AppSettings["menu-file"];
            if (fileName == null) fileName = "/menu.xml"; //默认在根目录
            fileName = AppContext.MapPath(fileName);
            return fileName;
        }

    }
}
