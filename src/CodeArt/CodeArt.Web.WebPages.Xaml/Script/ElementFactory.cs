using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    internal static class ElementFactory
    {
        public static T Create<T>(ScriptView view, DTObject element) where T : ScriptElement
        {
            var se = Activator.CreateInstance(typeof(T)) as ScriptElement;
            se.SetSource(view, element);
            return (T)se;
        }
    }

}
