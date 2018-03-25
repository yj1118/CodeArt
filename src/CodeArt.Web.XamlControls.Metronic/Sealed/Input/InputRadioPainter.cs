using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;
using CodeArt.Web.XamlControls.Bootstrap;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputRadioPainter : InputBasePainter
    {
        public InputRadioPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            StringBuilder uiParasCode = new StringBuilder(UIUtil.GetJSONMembers(node, "required", "validate", "onchange", "stringMode", "skin::'square-green'", "pulsate", "call:"));
            if (uiParasCode.Length > 0) uiParasCode.Append(",");
            uiParasCode.AppendFormat("items:{0}", UIUtil.GetOptionItems(node));

            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createRadio()"
                                           , uiParasCode.ToString()
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));


        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            FillControl(node, coreNode, brush);
        }

        protected virtual void FillControl(HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            var inline = node.GetAttributeValue("inline", "true");
            if (inline == "true")
            {
                brush.DrawLine("<div class=\"icheck-inline\">");
                brush.DrawLine("<label data-name=\"option\" data-proxy=\"{loops:'items',display:'',onbind:$$.wrapper.metronic.input.radio.bindOption}\"></label>");
                brush.DrawLine("</div>");
            }
            else
            {
                brush.DrawLine("<div class=\"icheck-list\">");
                brush.DrawLine("<label data-name=\"option\" data-proxy=\"{loops:'items',display:'',onbind:$$.wrapper.metronic.input.radio.bindOption}\"></label>");
                brush.Draw("</div>");
            }
        }

        protected override string GetCoreContainerProxyCode(HtmlNode node)
        {
            return "{give:new $$.component.databind()}";
        }

        public static readonly InputRadioPainter Instance = new InputRadioPainter();
    }
}
