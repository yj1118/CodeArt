using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Sealed;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.DTO;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    public class Progressbar : SealedControl
    {
        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            return Links;
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            var className = UIUtil.GetClassName(node, "progress progressbar");
            SealedPainter.CreateNodeCode(brush, "div", className, SealedPainter.GetStyleCode(node), GetProxyCode(node), (pageBrush) =>
            {
                pageBrush.DrawLine("<div class=\"progress-bar\" role=\"progressbar\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width: 0%\"></div>");
            }, () =>
            {
                return UIUtil.GetProperiesCode(node, "id", "name");
            });
        }

        private string GetProxyCode(HtmlNode node)
        {
            return UIUtil.GetProxyCode(this, node, "new $$metronic.progressbar()", string.Empty, UIUtil.GetJSONMembers(node, "onbind"));
        }


        public static LinkCode[] Links = new LinkCode[]
        {
            new LinkCode() { ExternalKey = "metronic:components-rounded.css", Origin = DrawOrigin.Header },
            //new LinkCode() { ExternalKey = "metronic:jquery.dataTables.js", Origin = DrawOrigin.Bottom },
        };

    }
}