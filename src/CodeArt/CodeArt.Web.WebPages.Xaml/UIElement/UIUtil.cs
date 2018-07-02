using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public static class UIUtil
    {
        public static string GetOptionItems(HtmlNode node)
        {
            StringBuilder code = new StringBuilder();
            code.Append("[");
            var groups = node.SelectNodesEx("core/items/group");
            if (groups.Count > 0)
            {
                foreach (HtmlNode group in groups)
                {
                    code.Append("{");
                    code.AppendFormat("'name':'{0}','items':[{1}]", group.GetAttributeValue("name", string.Empty), GetOptionsCode(group.SelectNodesEx("item")));
                    code.Append("},");
                }
            }
            else
            {
                var options = node.SelectNodesEx("core/items/item");
                code.Append(GetOptionsCode(options));
            }
            code.AppendFormat("]");
            return code.ToString();
        }

        private static string GetOptionsCode(HtmlNodeCollection options)
        {
            StringBuilder code = new StringBuilder();
            if (options.Count > 0)
            {
                foreach (HtmlNode item in options)
                {
                    code.Append("{");
                    code.AppendFormat("'value':'{0}','text':'{1}',disabled:{2}", item.GetAttributeValue("value", string.Empty), item.InnerHtml, item.GetAttributeValue("disabled", "false"));
                    code.Append("},");
                }
                code.Length--;
            }
            return code.ToString();
        }

        /// <summary>
        /// 获取样式名，如果指定了fullClass那么使用该值作为样式名
        /// </summary>
        /// <param name="node"></param>
        /// <param name="defaultClassName"></param>
        /// <returns></returns>
        public static string GetClassName(HtmlNode node, string defaultClassName)
        {
            string fullClass = node.GetAttributeValue("fullClass", string.Empty);
            if (!string.IsNullOrEmpty(fullClass)) return fullClass;
            string currentClass = node.GetAttributeValue("class", string.Empty);
            if (!string.IsNullOrEmpty(currentClass)) return string.Format("{0} {1}", defaultClassName, currentClass);
            return defaultClassName;
        }

        public static string GetProxyCode(UIElement e, HtmlNode node,string giveCode,string uiParasCode,string membersCode)
        {
            StringBuilder code = new StringBuilder("{");
            code.AppendFormat("give:{0}", giveCode);
            code.Append(",'" + node.LocalName() + "':true,uiParas:{");
            code.Append(uiParasCode);
            code.Append("}");
            if (!string.IsNullOrEmpty(membersCode)) code.AppendFormat(",{0}", membersCode);
            code.Append("}");
            var proxyCode = code.ToString();
            ProxyCodeExtend.TryFillCode(e, node, ref proxyCode);
            return proxyCode;
        }


        /// <summary>
        /// xxx:代表的是字符串字段，但是没有默认值
        /// xxx代表的是数字型字段，但是没有默认值
        /// xxx:aaa 代表的是默认值为aaa的字段，同时外界可以设置类型为数字的值的字段
        /// xxx::aaa 代表默认值是aaa的，同时外界可以设置值、类型为字符串的字段
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrNames"></param>
        /// <returns></returns>
        public static string GetJSONMembers(HtmlNode node, params string[] attrNames)
        {
            StringBuilder code = new StringBuilder();
            foreach (string nameValue in attrNames)
            {
                string name = null, value = null;
                bool isString = false;
                int colon = nameValue.IndexOf(':');
                if (NotStringMember(colon))
                {
                    name = nameValue;
                    value = node.GetAttributeValue(name, string.Empty);
                }
                else
                {
                    name = nameValue.Substring(0, colon);
                    if (NotDefaultValue(colon, nameValue))
                    {
                        //membreName:    代表字符串
                        value = node.GetAttributeValue(name, string.Empty);
                        isString = true;
                    }
                    else
                    { //membreName:memberValue    代表自定义键值对
                        value = node.GetAttributeValue(name, string.Empty); //先从用户设置的找，如果找不到，那么用默认值
                        if (value.Length > 0)
                        {
                            //用户设置了参数
                            if (IsStringMember(colon, nameValue))
                            {
                                isString = true;
                            }
                        }
                        else
                        {
                            if (SetMemberType(colon, nameValue)) colon++; //如果指定了成员类型（也就是，一共写了两个:）
                            //使用系统设置的默认值
                            value = nameValue.Substring(colon + 1);
                        }
                    }
                }

                if (value.Length > 0)
                {
                    if (isString)
                        code.AppendFormat("'{0}':'{1}',", name, value);
                    else
                        code.AppendFormat("'{0}':{1},", name, value);
                }
            }
            if (code.Length > 0) code.Length--;
            return code.ToString();
        }

        private static bool NotStringMember(int colon)
        {
            return colon < 0;
        }

        private static bool NotDefaultValue(int colon, string nameValue)
        {
            return colon == nameValue.Length - 1;
        }

        private static bool IsStringMember(int colon, string nameValue)
        {
            return nameValue[colon + 1] == ':';
        }

        private static bool SetMemberType(int colon, string nameValue)
        {
            return nameValue[colon + 1] == ':';
        }

        public static void CheckProperties(HtmlNode node, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                var attr = node.Attributes[propertyName];
                if (attr == null) throw new WebException(node.OriginalName + "没有指定" + propertyName);
            }
        }


        public static string GetProperiesCode(HtmlNode node, params string[] attrNames)
        {
            StringBuilder code = new StringBuilder();
            foreach (string nameValue in attrNames)
            {
                string name = null, value = null;
                bool isString = false;
                int colon = nameValue.IndexOf(':');
                if (NotStringMember(colon))
                {
                    name = nameValue;
                    value = node.GetAttributeValue(name, string.Empty);
                }
                else
                {
                    name = nameValue.Substring(0, colon);
                    if (NotDefaultValue(colon, nameValue))
                    {
                        //membreName:    代表字符串
                        value = node.GetAttributeValue(name, string.Empty);
                        isString = true;
                    }
                    else
                    { //membreName:memberValue    代表自定义键值对
                        value = node.GetAttributeValue(name, string.Empty); //先从用户设置的找，如果找不到，那么用默认值
                        if (value.Length > 0)
                        {
                            //用户设置了参数
                            if (IsStringMember(colon, nameValue))
                            {
                                isString = true;
                            }
                        }
                        else
                        {
                            if (SetMemberType(colon, nameValue)) colon++; //如果指定了成员类型（也就是，一共写了两个:）
                            //使用系统设置的默认值
                            value = nameValue.Substring(colon + 1);
                        }
                    }
                }

                if (value.Length > 0)
                {
                    code.AppendFormat(" {0}=\"{1}\" ", name, value);
                }
            }
            return code.ToString();
        }

    }
}
