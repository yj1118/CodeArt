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
    internal abstract class InputBasePainter
    {
        protected InputBasePainter() { }

        public virtual void FillHtml(object obj, HtmlNode node, PageBrush brush)
        {
            UIUtil.CheckProperties(node, "type");
            if (node.Attributes["name"] == null) node.SetAttributeValue("name", node.GetAttributeValue("id", string.Empty));

            SealedPainter.CreateNodeCode(brush, "div", UIUtil.GetClassName(node, string.Format("input input-{0} form-group", node.GetAttributeValue("type", string.Empty))), SealedPainter.GetStyleCode(node), GetProxyCode(obj, node), (pageBrush) =>
              {
                  FillLabel(node, pageBrush);
                  FillBrowseContainer(node, pageBrush);
                  FillCoreContainer(obj as SealedControl, node, pageBrush);
                  FillHelp(node, pageBrush);
              }, () =>
              {
                  return UIUtil.GetProperiesCode(node, "id", "name", "data-name");
              });
        }

        protected virtual void FillLabel(HtmlNode node, PageBrush brush)
        {
            string id = node.GetAttributeValue("id", string.Empty);
            var labelNode = node.SelectSingleNodeEx("label");
            if (labelNode == null)
            {
                string forCode = string.IsNullOrEmpty(id) ? string.Empty : string.Format(" for=\"control_{0}\"", id);
                brush.DrawFormat("<label class=\"control-label sr-only\"{0} data-name=\"label\"></label>", forCode);
                return;
            }
            else
            {
                var remove = labelNode.GetAttributeValue("remove", "false");
                if (remove == "true") return; //显示指定移除label标签
                string forCode = string.IsNullOrEmpty(id) ? string.Empty : string.Format(" for=\"control_{0}\"", id);
                string className = LayoutUtil.GetClassName(labelNode, "control-label");
                brush.DrawFormat("<label class=\"{0}\"{1} data-name=\"label\">{2}</label>", UIUtil.GetClassName(labelNode, className), forCode, labelNode.InnerText);
                brush.DrawLine();
            }
        }

        protected virtual void FillHelp(HtmlNode node, PageBrush brush)
        {
            var helpNode = node.SelectSingleNodeEx("help");
            if (helpNode == null) return;
            string className = LayoutUtil.GetClassName(helpNode, "help-block");
            brush.DrawFormat("<span class=\"{0}\" data-name=\"help\">{1}</span>", UIUtil.GetClassName(helpNode, className), helpNode.InnerText);
            brush.DrawLine();
        }

        protected virtual void FillBrowseContainer(HtmlNode node, PageBrush brush)
        {
            var browseNode = node.SelectSingleNodeEx("browse");
            if (browseNode != null)
            {
                string className = LayoutUtil.GetClassName(browseNode, "input-browse");
                className = UIUtil.GetClassName(browseNode, className);

                string proxyCode = UIUtil.GetJSONMembers(browseNode, "only", "submit");
                if (!string.IsNullOrEmpty(proxyCode)) proxyCode = "{" + proxyCode + "}";

                SealedPainter.CreateNodeCode(brush, "div", className, string.Empty, proxyCode, (pageBrush) =>
                {
                    FillBrowse(node, browseNode, pageBrush);
                },
                () =>
                {
                    return "data-name='browseContainer'";
                });
                //html.AppendLine(code);
            }
        }

        protected virtual void FillBrowse(HtmlNode node, HtmlNode browseNode, PageBrush brush)
        {

        }

        protected virtual void FillCoreContainer(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            var hasBrowseNode = node.SelectSingleNodeEx("browse") != null;
            var coreNode = node.SelectSingleNodeEx("core");
            if (coreNode == null) throw new WebException("没有指定core节点");
            string className = string.Empty;
            string styleCode = hasBrowseNode ? "display:none;" : string.Empty;

            if (IsSetCoreSuffix(node, coreNode))
            {
                className = LayoutUtil.GetClassName(coreNode, "input-group input-extend");
                styleCode += "padding-left:15px; padding-right:15px;";
            }
            else
            {
                className = LayoutUtil.GetClassName(coreNode, "input-extend");
            }

            if (!string.IsNullOrEmpty(styleCode)) styleCode = string.Format(" style=\"{0}\"", styleCode);
            className = UIUtil.GetClassName(coreNode, className);
            SealedPainter.CreateNodeCode(brush, "div", className, styleCode, GetCoreContainerProxyCode(node), (pageBrush) =>
            {
                FillCore(obj, node, coreNode, pageBrush);
            },
            () =>
            {
                return "data-name='coreContainer'";
            });
            //html.AppendLine(code);
        }

        protected virtual string GetCoreContainerProxyCode(HtmlNode node)
        {
            return string.Empty;
        }

        /// <summary>
        /// 是否指定了core内部的前后缀
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected virtual bool IsSetCoreSuffix(HtmlNode node, HtmlNode coreNode)
        {
            return false;
        }

        protected abstract void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush);

        protected abstract string GetProxyCode(object obj, HtmlNode node);
    }
}
