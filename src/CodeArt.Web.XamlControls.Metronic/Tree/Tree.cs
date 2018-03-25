using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ComponentLoader(typeof(TreeLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Tree.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Tree : Control
    {
        static Tree()
        {
        }

        public Tree()
        {
            this.Inited += OnInited;
        }

        private void OnInited(object sender, object e)
        {
            this.RegisterScriptAction("LoadData", this.TreeLoadData);
            this.RegisterScriptAction("ChangedValue", this.ChangedValue);
        }

        private IScriptView TreeLoadData(ScriptView view)
        {
            if (this.LoadData == null) throw new XamlException("没有为组件Tree设置LoadData方法，无法加载数据");
            var sender = view.GetSender<TreeSE>();
            var data = this.LoadData(view, sender);
            return new DataView(data);
        }

        /// <summary>
        /// 加载数据的方法
        /// </summary>
        public Func<ScriptView, TreeSE, DTObject> LoadData = null;

        private IScriptView ChangedValue(ScriptView view)
        {
            if (this.Changed == null) return view;
            var sender = view.GetSender<TreeSE>();
            return this.Changed(view, sender);
        }

        /// <summary>
        /// 当值发生变化时触发
        /// </summary>
        public Func<ScriptView, TreeSE, ScriptView> Changed = null;
    }
}
