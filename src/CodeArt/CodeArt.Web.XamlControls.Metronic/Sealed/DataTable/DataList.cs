using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Sealed;
using CodeArt.Web.WebPages.Xaml;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    public class DataList : SealedControl
    {
        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            return DataTablePainter.Links;
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            DataListPainter.Instance.FillHtml(this, node, brush);
        }
    }
}