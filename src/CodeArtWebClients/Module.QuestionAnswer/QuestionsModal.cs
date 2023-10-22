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
    [TemplateCode("Template", "Module.QuestionAnswer.QuestionsModal.html,Module.QuestionAnswer")]
    public class QuestionsModal : Control
    {
        private DataTable questionsTable;

        public QuestionsModal()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            questionsTable = this.GetTemplateChild<DataTable>("questionsTable");
            questionsTable.LoadData = Load;
        }

        public ScriptView OnQuestionsSearch(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("questionsTable");
            grid.Load();

            return view;
        }

        private DTObject Load(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            arg.Transform("tag=>tagId");
            arg["Slim"] = true;

            var data = ServiceContext.Invoke("getQuestionPage", arg);

            return data;
        }


        static QuestionsModal()
        { }
    }
}
   