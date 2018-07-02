using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.ServiceModel;
using FormSE = CodeArt.Web.XamlControls.Metronic.FormSE;
using CodeArt.Util;
using CodeArt.Web.WebPages;


namespace Module.QuestionAnswer
{
    [TemplateCode("Template", "Module.QuestionAnswer.PaperMetadatasModal.html,Module.QuestionAnswer")]
    public class PaperMetadatasModal : Control
    {
        private DataTable paperMetadatas;
        private HtmlElement paperMetadataSearch;

        public PaperMetadatasModal()
        {
        }

     

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            paperMetadatas = this.GetTemplateChild<DataTable>("paperMetadatas");
            this.paperMetadatas.LoadData = LoadMetadatas;

            paperMetadataSearch = this.GetTemplateChild<HtmlElement>("paperMetadataSearch");
            paperMetadataSearch.RegisterScriptAction("OnPaperMetadataSearch", OnPaperMetadataSearch);
        }


        public ScriptView OnPaperMetadataSearch(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("paperMetadatas");
            grid.Load();
            return view;
        }

        private DTObject LoadMetadatas(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            var data = ServiceContext.Invoke("getPaperMetadataPage", arg);

            return data;
        }


        static PaperMetadatasModal()
        { }
    }
}
   