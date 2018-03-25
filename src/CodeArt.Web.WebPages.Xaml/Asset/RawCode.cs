using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 将未加工处理的代码原样输出
    /// </summary>
    [ComponentLoader(typeof(RawCodeLoader))]
    public class RawCode : CodeAsset
    {
        public string Code
        {
            get;
            set;
        }

        protected override string GetCode()
        {
            return this.Code;
        }
    }
}
