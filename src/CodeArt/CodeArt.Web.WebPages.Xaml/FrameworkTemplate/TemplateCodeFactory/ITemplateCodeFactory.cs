using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface ITemplateCodeFactory
    {
        /// <summary>
        /// 模板代码创建工厂
        /// </summary>
        /// <param name="obj">需要创建模板的对象</param>
        /// <param name="templatePropertyName">模板属性的名称</param>
        /// <returns></returns>
        string GetTemplateCode(object obj, string templatePropertyName);
    }
}
