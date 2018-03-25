using CodeArt.Web.WebPages.Xaml.Sealed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.DTO;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    
    public class Dropdown : SealedControl
    {
        protected override void OnInit(HtmlNode node)
        {
            base.OnInit(node);
            if (!InputDropdownPainter.IsSingleLevel(node))
            {
                //多级下拉框
                this.RegisterScriptAction("LoadOptions", this.LoadOptions);
                this.RegisterScriptAction("Changed", this.ValueChanged);
            }
        }

        #region 多级下拉框

        private IScriptView LoadOptions(ScriptView view)
        {
            if (this.LoadOptionsData == null) throw new XamlException("没有为组件设置LoadOptionsData方法，无法加载下拉数据");
            var sender = view.GetSender<DropdownSE>();
            var data = this.LoadOptionsData(view, sender);
            return new DataView(data);
        }

        /// <summary>
        /// 加载下拉数据的方法
        /// </summary>
        public Func<ScriptView, DropdownSE, DTObject> LoadOptionsData = null;

        private IScriptView ValueChanged(ScriptView view)
        {
            if (this.Changed == null) return view;
            var sender = view.GetSender<DropdownSE>();
            return this.Changed(view, sender);
        }

        /// <summary>
        /// 当值发生变化时触发，该方法目前仅用于多级下拉框的使用，单级请自行挂载事件
        /// </summary>
        public Func<ScriptView, DropdownSE, ScriptView> Changed = null;

        #endregion


        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            List<CodeAsset> assets = new List<CodeAsset>();
            assets.Add(new LinkCode() { ExternalKey = "metronic:wrapper.input.js", Origin = DrawOrigin.Bottom });
            assets.Add(new LinkCode() { ExternalKey = "metronic:select.js", Origin = DrawOrigin.Bottom });
            assets.Add(new LinkCode() { ExternalKey = "metronic:select2.js", Origin = DrawOrigin.Bottom });
            assets.Add(new LinkCode() { ExternalKey = "metronic:select.css", Origin = DrawOrigin.Header });
            assets.Add(new LinkCode() { ExternalKey = "metronic:select2.css", Origin = DrawOrigin.Header });

            return assets.ToArray();
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            node.SetAttributeValue("type", "dropdown");
            InputDropdownPainter.Instance.FillHtml(this, node, brush);
        }
    }
}
