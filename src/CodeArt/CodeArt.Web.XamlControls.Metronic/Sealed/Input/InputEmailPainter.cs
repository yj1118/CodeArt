using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputEmailPainter : InputTextPainter
    {
        public InputEmailPainter() { }

        public new static readonly InputEmailPainter Instance = new InputEmailPainter();
    }
}
