using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]

    public class FrameworkTemplateLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);
            var template = obj as FrameworkTemplate;
            if(template != null)
            {
                var xmlns = XmlnsDictionary.Current;
                xmlns.Each((xmln)=>
                {
                    objNode.SetAttributeValue(xmln.Key, xmln.Value);
                });

                template.TemplateCode = objNode.OuterHtml;
            }
        }
    }
}
