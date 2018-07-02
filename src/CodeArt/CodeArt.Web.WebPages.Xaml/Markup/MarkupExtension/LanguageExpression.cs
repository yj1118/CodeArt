using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    ///  语言表达式
    ///  Static xx:Strings.xxx,
    ///  xx是xmlns命名空间，例如 m,ps等，如果命名空间不存在对应的程序集，那么就是站点的资源文件
    ///  Strings是框架保留关键字，只有Strings才表示绑定语言资源文本
    /// </summary>
    internal class LanguageExpression : Expression
    {
        private ResourceManager _manager;
        private string _resourceKey;
        public LanguageExpression(Assembly assembly, string resourceKey)
        {
            if(assembly!= null)
                _manager = new ResourceManager(string.Format("{0}.Strings", assembly.GetName().Name), assembly);
            _resourceKey = resourceKey;
        }

        public override object GetValue(DependencyObject d, DependencyProperty dp)
        {
            return GetValue();
        }

        public override object GetValue(object o, string propertyName)
        {
            return GetValue();
        }

        private string GetValue()
        {
            if (_manager == null) //没有程序集信息，从站点资源目录中提取
                return LanguageResources.Get(_resourceKey);

            return _manager.GetString(_resourceKey, Language.Current);
        }


    }
}
