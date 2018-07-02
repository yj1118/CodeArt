using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Runtime;

using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.XamlControls.Metronic
{
    /// <summary>
    /// 专用于disk组件路径的绑定
    /// </summary>
    public class DiskAssetsPathBindingExpression : Expression
    {
        /// <summary>
        /// 
        /// </summary>
        public string Path { get; private set; }

        public DiskAssetsPathBindingExpression(string path)
        {
            this.Path = path;
        }

        public override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return GetValue(d);
        }

        public override object GetValue(object o, string propertyName)
        {
            return GetValue(o);
        }

        public override bool SetValue(DependencyObject d, DependencyProperty dp, object value)
        {
            return false;
        }

        public override bool SetValue(object o, string propertyName, object value)
        {
            return false;
        }

        private object GetValue(object o)
        {
            var cell = o as ITemplateCell;
            if (cell == null) throw new XamlException("disk路径绑定表达式应用的对象必须是" + typeof(ITemplateCell).FullName);
            var target = cell.BelongTemplate ==null ? null : cell.BelongTemplate.TemplateParent as Disk;
            if (target == null) throw new XamlException("disk路径绑定表达式应用的对象必须是" + typeof(Disk).FullName);
            return string.Format("{0}{1}", target.AssetsPath, this.Path);
        }

    }
}