using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.ServiceModel;
using FormSE = CodeArt.Web.XamlControls.Metronic.FormSE;
using CodeArt.Util;
using Module.QuestionAnswer;

namespace Module.QuestionAnswer
{
    [TemplateCode("Template", "Module.QuestionAnswer.Questions.html,Module.QuestionAnswer")]
    public partial class Questions : Control
    {
        public static readonly DependencyProperty MarkedCodeProperty = DependencyProperty.Register<string, Questions>("MarkedCode", () => { return string.Empty; });
        public string MarkedCode
        {
            get
            {
                return (string)GetValue(MarkedCodeProperty);
            }
            set
            {
                SetValue(MarkedCodeProperty, value);
            }
        }

        public static readonly DependencyProperty AddServiceProperty = DependencyProperty.Register<string, Questions>("AddService", () => { return "addQuestion"; });
        public string AddService
        {
            get
            {
                return (string)GetValue(AddServiceProperty);
            }
            set
            {
                SetValue(AddServiceProperty, value);
            }
        }


        public Questions()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.RegisterScriptAction("OnQuestionDelete", OnQuestionDelete, SweetAlert.GetDeleteConfirmEvent());
        }

        public ScriptView OnQuestionAdd(ScriptView view)
        {
            var sender = view.GetSender();
            var metadataId = sender.Data.MetadataId;

            var data = DTObject.Create();
            data["metadataId"] = metadataId;
            var form = view.GetElement<FormSE>("formQuestionAdd");
            form.Accept(data);

            var modal = view.GetElement<ModalSE>("questionAddDialog");
            modal.Open();

            return view;
        }

        public ScriptView OnSubmitQuestionAdd(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formQuestionAdd");
            var data = form.Data;
            if (!string.IsNullOrEmpty(this.MarkedCode)) data["MetadataMarkedCode"] = this.MarkedCode;
            ServiceContext.Invoke(this.AddService, data);

            form.Reset();
            data.Transform("~metadataId");
            form.Set(data);
            form.AlertSuccess("保存成功");

            Refresh(view, data.Dynamic.MetadataId);
            Paper.ClearMetadata();
            return view;
        }

        public ScriptView OnQuestionEdit(ScriptView view)
        {
            var sender = view.GetSender();
            var id = sender.Data.Id;

            var data = ServiceContext.InvokeDynamic("getQuestion", (arg) =>
            {
                arg.id = id;
            });

            var form = view.GetElement<FormSE>("formQuestionEdit");
            data.Dynamic.MetadataId = sender.Data.MetadataId;
            form.Set(data);

            var modal = view.GetElement<ModalSE>("questionEditDialog");
            modal.Open();

            return view;
        }

        public ScriptView OnSubmitQuestionEdit(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formQuestionEdit");
            var data = form.Data;

            ServiceContext.Invoke("updateQuestion", data);

            form.AlertSuccess("保存成功");
            Refresh(view, data.Dynamic.MetadataId);
            Paper.ClearMetadata();
            return view;
        }

        public ScriptView OnQuestionDelete(ScriptView view)
        {
            var sender = view.GetSender();
            var id = sender.Data.Id;
            var metadataId = sender.Data.MetadataId;
            var data = ServiceContext.InvokeDynamic("deleteQuestion", (arg) =>
            {
                arg.QuestionDefinitionId = id;
                arg.MetadataId = metadataId;
            });

            Refresh(view, metadataId);
            Paper.ClearMetadata();
            return view;
        }


        public Action<ScriptView, object> CustomizeRefresh;

        private void Refresh(ScriptView view, object metadataId)
        {
            if (CustomizeRefresh != null)
            {
                CustomizeRefresh(view, metadataId);
            }
            else
            {
                var data = ServiceContext.InvokeDynamic("getPaperMetadata", (arg) =>
                {
                    arg.Id = metadataId;
                });

                var grid = view.GetElement<DataTableSE>("list");
                grid.ReloadRow(data);
            }

            view.WriteCode("_questions.questionsSync();");
        }

        static Questions()
        { }

    }
}