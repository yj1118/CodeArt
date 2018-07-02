using System.Collections.Generic;
using System.Web;
using System.IO;
using System;
using System.Text.RegularExpressions;

using CodeArt.DTO;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;
using CodeArt.Util;


namespace CodeArt.Web.XamlControls.Metronic
{
    internal static class DatatableExtractor
    {
        public static bool Exist(DTObject arg)
        {
            return arg.Exist("datatable[pagination][page]");
        }

        public static DTObject Transform(DTObject arg)
        { 
            //{component,action,argument:{elements:[{id,name,metadata}]}}
            DTObject result = DTObject.CreateReusable();
            DTObject sender = DTObject.CreateReusable();
            foreach(var p in arg.GetDictionary())
            {
                var name = p.Key;
                var value = p.Value;
                switch(name)
                {
                    case "pagination[page]":
                        {
                            sender["metadata.page"] = value;
                            break;
                        }
                    case "pagination[pages]":
                        {
                            sender["metadata.pages"] = value;
                            break;
                        }
                    case "pagination[perpage]":
                        {
                            sender["metadata.perpage"] = value;
                            break;
                        }
                    case "pagination[total]":
                        {
                            sender["metadata.total"] = value;
                            break;
                        }
                    case "sort[field]":
                        {
                            sender["metadata.sort.field"] = value;
                            break;
                        }
                    case "sort[sort]":
                        {
                            sender["metadata.sort.sort"] = value;
                            break;
                        }
                    case "info[component]":
                        {
                            result["component"] = value;
                            break;
                        }
                    case "info[action]":
                        {
                            result["action"] = value;
                            break;
                        }
                    case "info[senderId]":
                        {
                            sender["id"] = value;
                            break;
                        }
                    case "info[senderName]":
                        {
                            sender["name"] = value;
                            break;
                        }
                    case "info[dates]":
                        {
                            sender["metadata.dates"] = GetArray(value);
                            break;
                        }
                    case "info[datetimes]":
                        {
                            sender["metadata.datetimes"] = GetArray(value);
                            break;
                        }
                    default:
                        {
                            FillQuery(sender, name, value);
                            break;
                        }
                }
            }
            result.SetObject("argument.sender", sender);
            return result;
        }

        private static string[] GetArray(object value)
        {
            var str = value.ToString();
            if (string.IsNullOrEmpty(str)) return Array.Empty<string>();
            return str.Split(',');
        }

        const string prefix = "query[";

        private static void FillQuery(DTObject sender, string name, object value)
        {
            if (name.StartsWith(prefix))
            {
                var parasName = getParasName(name);
                sender[parasName] = value;
            }
        }

        private static Func<string, string> getParasName = LazyIndexer.Init<string, string>((name)=>
        {
            var reg = new Regex(@"\[([^\]\[]+?)\]");
            var matches = reg.Matches(name);
            List<string> path = new List<string>(matches.Count);
            foreach(Match matche in matches)
            {
                var value = matche.Groups[1].Value;
                if (value == "query") continue;
                path.Add(value);
            }
            name = string.Join(".", path);
            return string.Format("metadata.paras.{0}", name);
        });


    }
}
