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
    internal class InputTextareaPainter : InputTextPainter
    {
        public InputTextareaPainter() { }

        protected override string CreateControlCode(HtmlNode node, HtmlNode coreNode, string propertyText)
        {
            string rows = node.GetAttributeValue("rows", "3");
            return string.Format("<textarea {0} rows=\"{1}\" style=\"resize: none;\"></textarea>", propertyText, rows);
        }

        public new static readonly InputTextareaPainter Instance = new InputTextareaPainter();
    }
}
