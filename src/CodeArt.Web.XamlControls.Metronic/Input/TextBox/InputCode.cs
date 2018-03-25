using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class InputCode : CodeAsset
    {
        public InputCode()
        {
            this.Origin = DrawOrigin.Current;
        }

        protected override string GetCode()
        {
            return string.Empty;
        }
    }
}