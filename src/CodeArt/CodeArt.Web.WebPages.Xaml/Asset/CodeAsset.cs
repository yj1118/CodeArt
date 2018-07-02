using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 代码形式的资产
    /// </summary>
    public abstract class CodeAsset : UIElement
    {

        public virtual DrawOrigin Origin
        {
            get;
            set;
        }

        protected abstract string GetCode();


        protected override void Draw(PageBrush brush)
        {
            if(this.Origin == DrawOrigin.Header || this.Origin == DrawOrigin.Bottom)
            {
                brush.Backspace();
            }
            brush.Draw(GetCode(), this.Origin);
        }
    }
}
