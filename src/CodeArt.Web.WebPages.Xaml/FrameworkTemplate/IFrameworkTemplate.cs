using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IFrameworkTemplate
    {
        DependencyObject GetChild(string childName);


        /// <summary>
        /// 应用模板渲染
        /// </summary>
        /// <param name="templateParent">要应用此模板的元素</param>
        /// <returns></returns>
        void Render(PageBrush brush);
    }
}
