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

namespace Module.WebUI.Xaml.Pages
{
    [TemplateCode("Template", "Module.WebUI.Xaml.Pages.Role.Template.html,Module.WebUI.Xaml")]
    [TemplateCodeFactory("Template", typeof(DefaultTemplateCodeFactory))]
    public class Role : CodeArt.Web.XamlControls.Metronic.Page
    {
        public Role()
        {
        }

        private DataTable list;
        private DataTable fds;
        private HtmlElement search;
        private Button edit;
        private Button add;
        private Button deletes;
        private Button submitAdd;
        private Button submitEdit;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            list = this.GetTemplateChild<DataTable>("list");
            fds = this.GetTemplateChild<DataTable>("fds");
            search = this.GetTemplateChild<HtmlElement>("search");
            edit = this.GetTemplateChild<Button>("edit");
            add = this.GetTemplateChild<Button>("add");
            deletes = this.GetTemplateChild<Button>("deletes");
            submitAdd = this.GetTemplateChild<Button>("submitAdd"); 
            submitEdit = this.GetTemplateChild<Button>("submitEdit");

            this.list.LoadData = LoadRoles;
            this.fds.LoadData = LoadFDs;
            this.deletes.RegisterScriptAction("OnDeletes", OnDeletes, DataTableSE.GetDeletesConfirmEvent("list"));
            this.ScriptCallback += OnScriptCallback;
        }

        private IScriptView OnScriptCallback(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            grid.Load();
            return view;
        }

        private DTObject LoadFDs(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            var data = ServiceContext.Invoke("getPermissionPage", arg);
            return data;
        }

        private DTObject LoadRoles(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            var data = ServiceContext.Invoke("getRolePage", arg);
            return data;
        }

        public ScriptView OnSearch(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            grid.Load();
            return view;
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
            data.Transform("permissions=>permissionIds");

            ServiceContext.Invoke("addRole", data);
            form.Reset();
            form.AlertSuccess(Strings.SavedSuccessfully);

            var grid = view.GetElement<DataTableSE>("list");
            grid.Reload();

            return view;
        }

        public ScriptView OnEdit(ScriptView view)
        {
            var sender = view.GetSender();
            var id = sender.Data.Id;

            var data = ServiceContext.InvokeDynamic("getRole", (arg) =>
            {
                arg.id = id;
            });
            data.Transform("permissions.id=>value");
            data.Transform("permissions.name=>text");

            var form = view.GetElement<FormSE>("formEdit");
            form.Set(data);

            var modal = view.GetElement<ModalSE>("editDialog");
            modal.SetTitle(string.Format(Strings.EditRole, data.Dynamic.name));
            modal.Open();

            return view;
        }

        public ScriptView OnSubmitEdit(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formEdit");
            var data = form.Data;
            data.Transform("permissions=>permissionIds");

            ServiceContext.Invoke("updateRole", data);

            MenuHelper.RemoveAllMenuCode();

            var modal = view.GetElement<ModalSE>("editDialog");
            modal.Close();

            var grid = view.GetElement<DataTableSE>("list");
            grid.Reload();

            return view;
        }

        public ScriptView OnDeletes(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            var items = grid.SelectedValues;
            ServiceContext.Invoke("deleteRoles", items.ToReusableObject("ids"));

            MenuHelper.RemoveAllMenuCode();

            grid.Reload(items);
            return view;
        }


    }
}
   