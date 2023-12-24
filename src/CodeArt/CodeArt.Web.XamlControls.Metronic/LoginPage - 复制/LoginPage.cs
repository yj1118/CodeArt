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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.LoginPage.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class LoginPage : Page
    {
        public static DependencyProperty LogoProperty { get; private set; }
        
        public static DependencyProperty LoginTitleProperty { get; private set; }

        public static DependencyProperty RegisterButtonTextProperty { get; private set; }

        public static DependencyProperty RegisterUrlProperty { get; private set; }

        public static DependencyProperty SuccessUrlProperty { get; private set; }

        public static DependencyProperty ForgetPasswordUrlProperty { get; private set; }


        static LoginPage()
        {
            var logoMetadata = new PropertyMetadata(() => { return string.Empty; },OnLogoChanged);
            LogoProperty = DependencyProperty.Register<string, LoginPage>("Logo", logoMetadata);

            var loginTitleMetadata = new PropertyMetadata(() => { return string.Empty; });
            LoginTitleProperty = DependencyProperty.Register<string, LoginPage>("LoginTitle", loginTitleMetadata);

            var successUrlMetadata = new PropertyMetadata(() => { return string.Empty; });
            SuccessUrlProperty = DependencyProperty.Register<string, LoginPage>("SuccessUrl", successUrlMetadata);

            var registerUrlMetadata = new PropertyMetadata(() => { return string.Empty; });
            RegisterUrlProperty = DependencyProperty.Register<string, LoginPage>("RegisterUrl", registerUrlMetadata);

            var registerButtonTextMetadata = new PropertyMetadata(() => { return string.Empty; });
            RegisterButtonTextProperty = DependencyProperty.Register<string, LoginPage>("RegisterButtonText", registerButtonTextMetadata);

            var forgetPasswordUrlMetadata = new PropertyMetadata(() => { return string.Empty; });
            ForgetPasswordUrlProperty = DependencyProperty.Register<string, LoginPage>("ForgetPasswordUrl", forgetPasswordUrlMetadata);
        }

        private static void OnLogoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as LoginPage).OnLogoChanged(e);
        }

        protected virtual void OnLogoChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateLogoVisibility();
        }

        private void UpdateLogoVisibility()
        {
            if (this.Template == null) return;
            var logo = this.Logo;
            var img = this.Template.GetChild("logo") as UIElement;
            img.Visibility = string.IsNullOrEmpty(logo) ? Visibility.Collapsed : Visibility.Visible;
        }

        public string Logo
        {
            get
            {
                return GetValue(LogoProperty) as string;
            }
            set
            {
                SetValue(LogoProperty, value);
            }
        }

        /// <summary>
        /// 登录显示的标题
        /// </summary>
        public string LoginTitle
        {
            get
            {
                return GetValue(LoginTitleProperty) as string;
            }
            set
            {
                SetValue(LoginTitleProperty, value);
            }
        }

        /// <summary>
        /// 注册按钮显示的文本
        /// </summary>
        public string RegisterButtonText
        {
            get
            {
                return GetValue(RegisterButtonTextProperty) as string;
            }
            set
            {
                SetValue(RegisterButtonTextProperty, value);
            }
        }

        /// <summary>
        /// 登录成功后的跳转地址
        /// </summary>
        public string SuccessUrl
        {
            get
            {
                return GetValue(SuccessUrlProperty) as string;
            }
            set
            {
                SetValue(SuccessUrlProperty, value);
            }
        }

        /// <summary>
        /// 注册页地址
        /// </summary>
        public string RegisterUrl
        {
            get
            {
                return GetValue(RegisterUrlProperty) as string;
            }
            set
            {
                SetValue(RegisterUrlProperty, value);
            }
        }

        public string ForgetPasswordUrl
        {
            get
            {
                return GetValue(ForgetPasswordUrlProperty) as string;
            }
            set
            {
                SetValue(ForgetPasswordUrlProperty, value);
            }
        }


        public LoginPage()
        {
            this.RegisterScriptAction("Login", Login);
        }

        protected virtual IScriptView Login(ScriptView view)
        {
            var form = view.GetElement<FormSE>("loginForm");
            var data = form.Data;
            data["ip"] = HttpContext.Current.Request.UserHostAddress;

            var handler = ModuleController.GetHandler("login");
            DTObject dto = handler.Process(data);

            var success = dto.GetValue<bool>("success");
            if (success)
            {
                form.Reset();
                view.Redirect(this.SuccessUrl);
            }
            else
            {
                view.GetElement("alertText").SetText(Strings.WrongUserNamePassword);
                view.GetElement("alert").Visible();
            }
            return view;
        }
    }
}
