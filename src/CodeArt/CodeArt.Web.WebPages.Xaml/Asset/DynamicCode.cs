using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class DynamicCode : ContentControl
    {
        public virtual DrawOrigin Origin
        {
            get;
            set;
        }

        protected override void Draw(PageBrush brush)
        {
            if (this.Origin == DrawOrigin.Header || this.Origin == DrawOrigin.Bottom)
            {
                brush.Backspace();
                var code = GetCode();
                brush.Draw(code, this.Origin);
            }
            else base.Draw(brush);
        }

        private string GetCode()
        {
            string code = string.Empty;
            using (var temp = XamlUtil.BrushPool.Borrow())
            {
                var newBrush = temp.Item;
                this.Content.Render(newBrush);
                code = newBrush.GetCode();
            }
            return code;
        }
    }
}
