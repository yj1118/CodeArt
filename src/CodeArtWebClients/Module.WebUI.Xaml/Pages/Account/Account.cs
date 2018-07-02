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

namespace Module.WebUI.Xaml.Pages
{
    [TemplateCode("Template", "Module.WebUI.Xaml.Pages.Account.Template.html,Module.WebUI.Xaml")]
    [TemplateCodeFactory("Template", typeof(DefaultTemplateCodeFactory))]
    public class Account : CodeArt.Web.XamlControls.Metronic.Page
    {
        public Account()
        {
        }

        private DataTable list;
        private DataTable roles;
        private HtmlElement search;
        private Button edit;
        private Button add;
        private Button deletes;
        private Button submitAdd;
        private Button submitEdit;
        private Button roleSearch;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            list = this.GetTemplateChild<DataTable>("list");
            roles = this.GetTemplateChild<DataTable>("roles");
            search = this.GetTemplateChild<HtmlElement>("search");
            edit = this.GetTemplateChild<Button>("edit");
            add = this.GetTemplateChild<Button>("add");
            deletes = this.GetTemplateChild<Button>("deletes");
            submitAdd = this.GetTemplateChild<Button>("submitAdd"); 
            submitEdit = this.GetTemplateChild<Button>("submitEdit");
            roleSearch = this.GetTemplateChild<Button>("roleSearch");

            this.list.LoadData = LoadAccounts;
            this.roles.LoadData = LoadRoles;
            this.deletes.RegisterScriptAction("OnDeletes", OnDeletes, DataTableSE.GetDeletesConfirmEvent("list"));
            this.ScriptCallback += OnScriptCallback;
        }

        private IScriptView OnScriptCallback(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("list");
            grid.Load();
            return view;
        }

        private DTObject LoadAccounts(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            var data = ServiceContext.Invoke("getAccountPage", arg);
            data.Transform("rows.LoginTime=status.loginInfo.lastTime");
            data.Transform("rows.LoginIp=status.loginInfo.lastIp");
            data.Transform("rows.LoginTimes=status.loginInfo.total");
            data.Transform("rows.IsEnabled=status.isEnabled", (v) =>
            {
                var isEnabled = (bool)v;
                return isEnabled ? Strings.Enable : Strings.Disable;
            });
            data.Transform("!rows.status");
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
            data.Transform("isEnabled=isEnabled", (t) =>
            {
                return DataUtil.ToValue<bool>(t);
            });
            data.Transform("roles=>roleIds");

            ServiceContext.Invoke("addUser", data);
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

            var data = ServiceContext.InvokeDynamic("getAccount", (arg) =>
            {
                arg.id = id;
            });
            data.Transform("roles.id=>value");
            data.Transform("roles.name=>text");
            data.Transform("rpassword=password");
            data.Transform("isEnabled=status.isEnabled");

            var form = view.GetElement<FormSE>("formEdit");
            form.Set(data);

            var modal = view.GetElement<ModalSE>("editDialog");
            modal.SetTitle(string.Format(Strings.ModifyAccountInformationTip, data.Dynamic.name));
            modal.Open();

            return view;
        }

        public ScriptView OnSubmitEdit(ScriptView view)
        {
            var form = view.GetElement<FormSE>("formEdit");
            var data = form.Data;
            data.Transform("isEnabled=isEnabled", (t) =>
            {
                return DataUtil.ToValue<bool>(t);
            });
            data.Transform("roles=>roleIds");

            ServiceContext.Invoke("updateAccount", data);

            MenuHelper.RemoveMenuCode(data.Dynamic.Id.ToString());

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
            ServiceContext.Invoke("deleteUsers", items.ToReusableObject("ids"));

            var ids = items.ToArray<string>();
            MenuHelper.RemoveMenuCode(ids);

            grid.Reload(items);
            return view;
        }


        private DTObject LoadRoles(ScriptView view, DataTableSE element)
        {
            var arg = element.GetQuery();
            var data = ServiceContext.Invoke("getRolePage", arg);
            return data;
        }

        public ScriptView OnRoleSearch(ScriptView view)
        {
            var grid = view.GetElement<DataTableSE>("roles");
            grid.Load();
            return view;
        }


    }
}
   