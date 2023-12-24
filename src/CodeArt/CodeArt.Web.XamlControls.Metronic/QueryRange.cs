using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;


namespace CodeArt.Web.XamlControls.Metronic
{
    public abstract class QueryRange : ContentControl
    {
        /// <summary>
        /// 自定义按钮的区域
        /// </summary>
        public readonly static DependencyProperty ButtonProperty = DependencyProperty.Register<UIElementCollection, ContentControl>("Button", () => { return new UIElementCollection(); });

        public UIElementCollection Button
        {
            get
            {
                return GetValue(ButtonProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ButtonProperty, value);
            }
        }

        public readonly static DependencyProperty SearchActionProperty = DependencyProperty.Register<string, QueryRange>("SearchAction", () => { return "OnSearch"; });

        public string SearchAction
        {
            get
            {
                return GetValue(SearchActionProperty) as string;
            }
            set
            {
                SetValue(SearchActionProperty, value);
            }
        }


        public readonly static DependencyProperty ShowDefaultButtonProperty = DependencyProperty.Register<bool, QueryRange>("ShowDefaultButton", () => { return true; });

        public bool ShowDefaultButton
    {
            get
            {
                return (bool)GetValue(ShowDefaultButtonProperty);
            }
            set
            {
                SetValue(ShowDefaultButtonProperty, value);
            }
        }

        public abstract string CompontentName
        {
            get;
        }

        protected override void Draw(PageBrush brush)
        {
            if (this.Content.Count == 0) return;
            brush.DrawFormat("<div class=\"m-form m-form--label-align-right m--margin-top-20 m--margin-bottom-20 c-{0}Query\">", this.CompontentName);
            brush.DrawLine("<div class=\"row align-items-center\">");
            brush.DrawLine("<div class=\"col-sm-12\">");
            brush.DrawLine("<div class=\"form-group m-form__group row align-items-center\">");
            DrawMembers(brush);
            DrawButton(brush);
            brush.DrawLine("</div>");
            brush.DrawLine("</div>");
            brush.DrawLine("</div>");
            brush.Draw("</div>");
        }

        private void DrawMembers(PageBrush brush)
        {
            var cls = string.Empty;//1个成员
            switch (this.Content.Count)
            {
                case 1: cls = "col-xl-6 col-md-12"; break;
                case 2: cls = "col-xl-4 col-lg-6 col-md-12"; break;
                default:
                    cls = "col-xl-3 col-lg-6 col-md-12"; break;
            }
            foreach (UIElement item in this.Content)
            {
                brush.DrawFormat("<div class=\"{0}\">", cls);
                brush.DrawLine();
                item.Render(brush);
                brush.DrawLine("<div class=\"m--margin-bottom-10\"></div>");
                brush.DrawLine("</div>");
            }
        }

        private void DrawButton(PageBrush brush)
        {
            var cls = string.Empty;
            if (this.Content.Count == 1) cls = "col-xl-6 col-md-12 m--align-right";
            else if (this.Content.Count == 2) cls = "col-xl-4 col-md-12 m--align-right";
            else
            {
                var xlCol = (4 - this.Content.Count % 4) * 3;
                var lgCol = (2 - this.Content.Count % 2) * 6;
                cls = string.Format("col-xl-{0} col-lg-{1} col-md-12 m--align-right", xlCol, lgCol);
            }

            brush.DrawFormat("<div class=\"{0}\">", cls);
            brush.DrawLine();
            var noneCls = this.Content.Count == 2 ? "d-xl-none" : "d-lg-none";
            brush.DrawFormat("<div class=\"m-separator m-separator--dashed {0}\"></div>", noneCls);
            brush.DrawLine();
            if (this.Button.Count == 0 && this.ShowDefaultButton) //如果没有指定自定义按钮，并且要求显示默认按钮，那么输出
            {
                brush.DrawLine("<a href=\"javascript:;\" data-proxy=\"{invoke:{events:[{client:'click',server:'" + this.SearchAction + "',view:'',option:{}}]}}\" class=\"btn btn-primary m-btn m-btn--custom m-btn--icon m-btn--air\">");
                brush.DrawFormat("<span><i class=\"la la-search\"></i><span>{0}</span></span>", Strings.Search);
            }
            else
            {
                //自定义button
                this.Button.Render(brush);
            }
            brush.DrawLine();
            brush.DrawLine("</a>");
            brush.DrawLine("<div class=\"m--margin-bottom-15\"></div>");
            brush.DrawLine("</div>");
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Button.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName), this.Button.GetActionElement(actionName));
        }

    }
}
