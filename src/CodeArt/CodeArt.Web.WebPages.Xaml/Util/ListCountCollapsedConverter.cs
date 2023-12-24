﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class ListCountCollapsedConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var list = value as IEnumerable;
            foreach(var t in list)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public readonly static ListCountCollapsedConverter Instance = new ListCountCollapsedConverter();
    }
}