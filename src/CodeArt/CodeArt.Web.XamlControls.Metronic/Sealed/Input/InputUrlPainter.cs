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
    internal class InputUrlPainter : InputTextPainter
    {
        public InputUrlPainter() { }

        public new static readonly InputUrlPainter Instance = new InputUrlPainter();
    }
}
