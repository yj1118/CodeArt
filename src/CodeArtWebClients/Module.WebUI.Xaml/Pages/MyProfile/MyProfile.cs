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
using CodeArt;
using CodeArt.Web.WebPages;

namespace Module.WebUI.Xaml.Pages
{
    [TemplateCode("Template", "Module.WebUI.Xaml.Pages.MyProfile.Template.html,Module.WebUI.Xaml")]
    [TemplateCodeFactory("Template", typeof(DefaultTemplateCodeFactory))]
    public class MyProfile : CodeArt.Web.XamlControls.Metronic.Page
    {
        public static readonly DependencyProperty DirectoryIdProperty = DependencyProperty.Register<Guid, MyProfile>("DirectoryId", () => { return string.Empty; });
        /// <summary>
        /// 
        /// </summary>
        public Guid DirectoryId
        {
            get
            {
                return (Guid)GetValue(DirectoryIdProperty);
            }
            set
            {
                SetValue(DirectoryIdProperty, value);
            }
        }

        public MyProfile()
        {
        }



        private Button baseSubmit;
        private Button accountSubmit;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            baseSubmit = this.GetTemplateChild<Button>("baseSubmit");
            baseSubmit.RegisterScriptAction("OnBaseSubmit", OnBaseSubmit);

            accountSubmit = this.GetTemplateChild<Button>("accountSubmit");
            accountSubmit.RegisterScriptAction("OnAccountSubmit", OnAccountSubmit);

            this.DataContext = this;
            this.ScriptCallback += OnScriptCallback;

        }

        private IScriptView OnScriptCallback(ScriptView view)
        {
            Init(view);
            return view;
        }

        private void Init(ScriptView view)
        {
            var data = ServiceContext.InvokeDynamic("getUser", (arg) => { arg.Id = Principal.Id; });
            data.Transform("email=account.email;mobileNumber=account.mobileNumber");

            var form = view.GetElement<FormSE>("baseForm");
            form.Set(data);

            var file = this.Template.GetFile("/Pages/MyProfile/images/photo.png");
            data.Dynamic.PhotoUrl = data.Dynamic.Photo != null ? ImageUtil.GetDynamicUrl(data.Dynamic.Photo.StoreKey, 192, 192, 2)
                                                                : file.VirtualPath;

            var card = view.GetElement<HtmlEelementSE>("card");
            card.Bind(data);
        }


        public override void OnLoad()
        {
            base.OnLoad();
            this.DirectoryId = VirtualFileUtil.GetRootDirectory().Id;
        }

        public ScriptView OnBaseSubmit(ScriptView view)
        {
            var form = view.GetElement<FormSE>("baseForm");
            var data = form.Data;
            data["id"] = Principal.Id;

            ServiceContext.Invoke("updateUser", data);

            Init(view);

            form.AlertSuccess(Strings.SuccessfulOperation);

            return view;
        }

        public ScriptView OnAccountSubmit(ScriptView view)
        {
            var form = view.GetElement<FormSE>("accountForm");
            var data = form.Data;

            if (data.Dynamic.newPassword != data.Dynamic.rPassword)
            {
                throw new UserUIException(Strings.PasswordTip2);
            }

            data["id"] = Principal.Id;

            ServiceContext.Invoke("updatePassword", data);

            Init(view);

            form.AlertSuccess(Strings.SuccessfulOperation);

            return view;
        }

    }
}
   