using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface IStyle
    {
        XamlStyle BasedOn { get; set; }

        SetterCollection Setters { get; }

        Type TargetType { get; }

        /// <summary>
        /// 对目标对象应用样式
        /// </summary>
        /// <param name="target"></param>
        void Apply(DependencyObject target);
    }
}
