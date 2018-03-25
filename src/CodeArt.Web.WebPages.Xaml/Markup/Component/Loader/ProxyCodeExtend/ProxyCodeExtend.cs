using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Runtime;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    public static class ProxyCodeExtend
    {
        /// <summary>
        /// 填充proxy代码
        /// </summary>
        public static bool TryFillCode(UIElement e, HtmlNode objNode,ref string proxyCode)
        {
            if (e == null) return false;
            var extendCode = GetCode(e, objNode);
            if (string.IsNullOrEmpty(extendCode)) return false;

            if (string.IsNullOrEmpty(proxyCode))
            {
                proxyCode = "{" + extendCode + "}";
                return true;
            }

            proxyCode = proxyCode.Insert(proxyCode.Length - 1, "," + extendCode);
            return true;
        }

        /// <summary>
        /// 获取扩展的proxy代码
        /// </summary>
        private static string GetCode(UIElement e, HtmlNode objNode)
        {
            StringBuilder code = null;
            JoinCode(ref code, GetScriptEventCode(e, objNode));
            //JoinCode(ref code, GetViewCode(e, objNode));
            if (code != null) code.Length--;
            return code == null ? null : code.ToString();
        }

        private static void JoinCode(ref StringBuilder code, string itemCode)
        {
            if (string.IsNullOrEmpty(itemCode)) return;
            if (code == null) code = new StringBuilder();
            code.AppendFormat("{0},",itemCode);
        }

        private static string GetScriptEventCode(UIElement e, HtmlNode objNode, ScriptEventDefine[] eventDefines)
        {
            string view = string.Empty;
            string name = string.Empty;

            //invoke:{component:'xxx',events:[{client:'click',server:'save',view:''}]}
            StringBuilder invokeCode = null;
            foreach (var define in eventDefines)  //client="server" 客户端事件对应的执行服务器端的处理程序名称
            {
                var server = objNode.GetAttributeValue(define.OriginalName, string.Empty); //获得定义的服务器端处理程序名称
                const string defaultOption = "{}";
                var option = defaultOption;
                var dotPos = server.IndexOf(',');
                if (dotPos > -1)
                {
                    var temp = server;
                    server = temp.Substring(0, dotPos);
                    option = temp.Substring(dotPos + 1);
                }

                if (!string.IsNullOrEmpty(server))
                {
                    if (invokeCode == null)
                    {
                        view = objNode.GetAttributeValue("view", string.Empty);
                        name = objNode.GetAttributeValue("name", string.Empty);
                        if (string.IsNullOrEmpty(name)) throw new XamlException("由于指定了远程脚本方法，组件必须指定Name");

                        invokeCode = new StringBuilder();
                        invokeCode.Append("invoke:{");
                        invokeCode.AppendFormat("component:'{0}',events:[", name);
                    }
                    invokeCode.Append("{");
                    invokeCode.AppendFormat("client:'{0}',server:'{1}',view:'{2}',option:{3}", define.ClientName, server, TidyView(view), option);
                    invokeCode.Append("},");

                    RemoveAttribute(e, objNode, define.OriginalName);
                }
            }

            if (invokeCode != null)
            {
                invokeCode.Length--;
                invokeCode.Append("]}");

                return invokeCode.ToString();
            }
            return null;
        }

        /// <summary>
        /// 获取脚本事件的扩展代代码
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objNode"></param>
        public static string GetScriptEventCode(UIElement e, HtmlNode objNode)
        {
            var attr = ScriptEventAttribute.GetAttribute(e.ObjectType);
            var defines = attr.Defines;
            return GetScriptEventCode(e, objNode, defines);
        }

        public static string GetScriptEventCode(UIElement e, HtmlNode objNode,params string[] eventDefineCodes)
        {
            var attr = new ScriptEventAttribute(eventDefineCodes);
            var defines = attr.Defines;
            return GetScriptEventCode(e, objNode, defines);
        }

        private static void RemoveAttribute(UIElement e, HtmlNode objNode,string attrName)
        {
            e.Attributes.RemoveValue(attrName);
            objNode.RemoveAttribute(attrName);
        }

        //private static string GetViewCode(UIElement e, HtmlNode objNode)
        //{
        //    var view = objNode.GetAttributeValue("view", null);
        //    if (view == null) return string.Empty; //没有定义参与脚本视图
        //    view = TidyView(view);

        //    //string id = objNode.GetAttributeValue("id", string.Empty);
        //    //if (view != "_self" && string.IsNullOrEmpty(id))
        //    //    throw new XamlException("指定了view属性，必须同时指定组件的编号id,组件代码" + objNode.OuterHtml);

        //    var code = string.Format("view:'{0}'", view);
        //    RemoveAttribute(e, objNode, "view");
        //    return code;
        //}

        internal static string TidyView(string view)
        {
            if (view.Equals("true", StringComparison.OrdinalIgnoreCase)) return string.Empty;
            else if (view.Equals("false", StringComparison.OrdinalIgnoreCase)) return "none";
            return view;
        }

    }
}