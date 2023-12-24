﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal class AdminPageTemplateCodeFactory : ITemplateCodeFactory
    {

        public string GetTemplateCode(object obj, string templatePropertyName)
        {
            var page = obj as AdminPage;
            var theme = page.Theme;
            return _getTemplateCode(theme);
        }

        private static Func<string, string> _getTemplateCode = LazyIndexer.Init<string, string>((skin) =>
        {
            var url = string.Format("CodeArt.Web.XamlControls.Metronic.AdminPage.{0}.html,CodeArt.Web.XamlControls.Metronic", skin);
            var code = AssemblyResource.LoadText(url);
            if (string.IsNullOrEmpty(code)) throw new XamlException("没有找到AdminPage的 " + skin + "皮肤");
            return code;
        });


        public static readonly AdminPageTemplateCodeFactory Instance = new AdminPageTemplateCodeFactory();
    }
}
