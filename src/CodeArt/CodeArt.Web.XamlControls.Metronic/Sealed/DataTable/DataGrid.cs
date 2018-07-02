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
    public class DataGrid : SealedControl
    {
        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            return DataTablePainter.Links;
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            DataGridPainter.Instance.FillHtml(this, node, brush);
        }

        public override void OnInit()
        {
            base.OnInit();
            this.RegisterScriptAction("Load", this.Load);
        }

        private IScriptView Load(ScriptView view)
        {
            if (this.LoadData == null) throw new XamlException("没有为组件设置LoadData方法，无法加载数据");
            var sender = view.GetSender<DataGridSE>();
            var data = this.LoadData(view, sender);
            return new DataView(data);
        }

        public Func<ScriptView, DataGridSE, DTObject> LoadData = null;
       


    }
}