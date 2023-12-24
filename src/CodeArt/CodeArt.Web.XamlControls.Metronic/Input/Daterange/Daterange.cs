﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;


namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Daterange.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Daterange : Input
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register<string, Daterange>("Placeholder", () => { return string.Empty; });

        public string Placeholder
        {
            get
            {
                return GetValue(PlaceholderProperty) as string;
            }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register<string, Daterange>("Format", () => { return string.Empty; });

        public string Format
        {
            get
            {
                return GetValue(FormatProperty) as string;
            }
            set
            {
                SetValue(FormatProperty, value);
            }
        }

    }
}
