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
    [TemplateCode("Template", "Module.QuestionAnswer.Metadata.html,Module.QuestionAnswer")]
    public partial class Metadata : Control
    {
        public Metadata()
        {
        }

        private DataTable list;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            list = this.GetTemplateChild<DataTable>("list");
            list.LoadData = Load;

            this.RegisterScriptAction("OnDeletes", OnDeletes, DataTableSE.GetDeletesConfirmEvent("list"));

            this.ScriptCallback += OnScriptCallback;
        }

        private IScriptView OnScriptCallback(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            grid.Load();
            return view;
        }

        public ScriptView OnDeletes(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            var items = grid.SelectedValues;
            ServiceContext.Invoke("deletePaperMetadatas", items.ToObject("ids"));
            grid.Reload(items);

            Paper.ClearMetadata();

            return view;
        }

        public ScriptView OnSearch(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            grid.Load();
            return view;
        }

        private DTObject Load(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            var data = ServiceContext.Invoke("getPaperMetadataPage", arg);

            return data;
        }

        public ScriptView OnAdd(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formAdd");
            form.Reset();

            var modal = view.GetElement<ModalSE>("addDialog");
            modal.Open();

            return view;
        }

        public ScriptView OnSubmitAdd(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formAdd");
            var data = form.Data;

            ServiceContext.Invoke("addPaperMetadata", data);
            form.Reset();
            form.AlertSuccess("保存成功");

            var grid = view.GetElement<DataTableSE>("list");
            grid.Reload();

            Paper.ClearMetadata();

            return view;
        }

        public ScriptView OnEdit(ScriptView view)
        {
            var sender = view.GetSender();
            var id = sender.Data.Id;

            var data = ServiceContext.InvokeDynamic("getPaperMetadata", (arg) =>
            {
                arg.id = id;
            });

            var form = view.GetElement<FormSE>("formEdit");
            form.Set(data);

            var modal = view.GetElement<ModalSE>("editDialog");
            modal.Open();

            return view;
        }

        public ScriptView OnSubmitEdit(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formEdit");
            var data = form.Data;

            ServiceContext.Invoke("updatePaperMetadata", data);

            var modal = view.GetElement<ModalSE>("editDialog");
            modal.Close();

            var grid = view.GetElement<DataTableSE>("list");
            grid.Reload();

            Paper.ClearMetadata();

            return view;
        }

        public ScriptView GetQuestions(ScriptView view)
        {
            var sender = view.GetSender();
            var metadataId = sender.Data.Id;

            var data = ServiceContext.InvokeDynamic("getQuestions", (arg) =>
            {
                arg["metadataId"] = metadataId;
            });
            data.SetValue("metadataId", metadataId);
            data.Each("rows", (row) =>
            {
                row.Dynamic.MetadataId = metadataId;
            });
            view.Output(data);
            return view;
        }

        static Metadata()
        { }

    }
}