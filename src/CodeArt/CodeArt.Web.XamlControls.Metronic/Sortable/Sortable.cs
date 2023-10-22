using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Sortable.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Sortable : Control
    {
        public override void OnInit()
        {
            base.OnInit();
            this.RegisterScriptAction("Load", this.Load);
        }

        private IScriptView Load(ScriptView view)
        {
            if (this.LoadData == null) throw new XamlException(Strings.NoLoadDataMethod);
            var sender = view.GetSender<SortableSE>();
            var data = this.LoadData(view, sender);
            view.Output(data);
            return view;
        }

        public Func<ScriptView, SortableSE, DTObject> LoadData = null;

        private IScriptView Sort(ScriptView view)
        {
            if (this.SaveData == null) throw new XamlException("没有实现SaveData方法");
            var sender = view.GetSender<SortableSE>();
            this.SaveData(view, sender);
            return view;
        }

        public Action<ScriptView, SortableSE> SaveData = null;

    }
}
