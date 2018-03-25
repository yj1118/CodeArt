using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public class DefaultTemplateCodeFactory : ITemplateCodeFactory
    {

        public string GetTemplateCode(object obj, string templatePropertyName)
        {
            var objType = XamlUtil.GetObjectType(obj);
            return _getTemplateCode(objType)(templatePropertyName);
        }

        private static Func<Type, Func<string, string>> _getTemplateCode = LazyIndexer.Init<Type, Func<string, string>>((objType) =>
        {
            return LazyIndexer.Init<string, string>((templatePropertyName) =>
            {
                var code = TemplateCodeAttribute.GetAttribute(objType, templatePropertyName);
                if (code == null) return null;
                return code.GetXaml();
            });
        });


        public static readonly DefaultTemplateCodeFactory Instance = new DefaultTemplateCodeFactory();
    }
}
