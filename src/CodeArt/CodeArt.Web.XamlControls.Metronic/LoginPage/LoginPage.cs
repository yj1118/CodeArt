using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.ModuleNest;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;


namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.LoginPage.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class LoginPage : CodeArt.Web.WebPages.Xaml.Controls.Page
    {
        public static DependencyProperty LogoProperty { get; private set; }
        
        public static DependencyProperty LoginTitleProperty { get; private set; }

        public static DependencyProperty RegisterButtonTextProperty { get; private set; }

        public static DependencyProperty SuccessUrlProperty { get; private set; }

        public static DependencyProperty RegisterTitleProperty { get; private set; }

        public static readonly DependencyProperty BackgroundImageProperty = DependencyProperty.Register<string, LoginPage>("BackgroundImage", () => { return string.Empty; });

        public static readonly DependencyProperty RegisterUrlProperty = DependencyProperty.Register<string, LoginPage>("RegisterUrl", () => { return string.Empty; });

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


        static LoginPage()
        {
            var logoMetadata = new PropertyMetadata(() => { return string.Empty; },OnLogoChanged);
            LogoProperty = DependencyProperty.Register<string, LoginPage>("Logo", logoMetadata);

            var loginTitleMetadata = new PropertyMetadata(() => { return string.Empty; });
            LoginTitleProperty = DependencyProperty.Register<string, LoginPage>("LoginTitle", loginTitleMetadata);

            var successUrlMetadata = new PropertyMetadata(() => { return string.Empty; });
            SuccessUrlProperty = DependencyProperty.Register<string, LoginPage>("SuccessUrl", successUrlMetadata);

            var registerButtonTextMetadata = new PropertyMetadata(() => { return string.Empty; });
            RegisterButtonTextProperty = DependencyProperty.Register<string, LoginPage>("RegisterButtonText", registerButtonTextMetadata);

            var registerTitleMetadata = new PropertyMetadata(() => { return string.Empty; });
            RegisterTitleProperty = DependencyProperty.Register<string, LoginPage>("RegisterTitle", registerTitleMetadata);

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

        public string BackgroundImage
        {
            get
            {
                return GetValue(BackgroundImageProperty) as string;
            }
            set
            {
                SetValue(BackgroundImageProperty, value);
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
        /// 登录或注册成功后的跳转地址
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
        /// 注册显示的标题
        /// </summary>
        public string RegisterTitle
        {
            get
            {
                return GetValue(RegisterTitleProperty) as string;
            }
            set
            {
                SetValue(RegisterTitleProperty, value);
            }
        }

        public LoginPage()
        {
            this.RegisterScriptAction("Login", Login);
            this.RegisterScriptAction("Register", Register);
        }

        protected virtual IScriptView Login(ScriptView view)
        {
            var form = view.GetElement<FormSE>("loginForm");
            var data = form.Data;
            data["ip"] = HttpContext.Current.Request.UserHostAddress;

            var success = SignIn(data);
            if (success)
            {
                view.Redirect(this.SuccessUrl);
            }
            else
            {
                view.WriteCode(string.Format("loginForm.proxy().showError('{0}');", Strings.WrongUserNamePassword));
            }
            return view;
        }

        private IScriptView Register(ScriptView view)
        {
            var form = view.GetElement<FormSE>("registerForm");
            var data = form.Data;
            var result = SignUp(data);
            if (result.Success)
            {
                view.WriteCode("SnippetLogin.displaySignInForm();");
                view.WriteCode(string.Format("$$('#loginForm').show('{0}');", Strings.OperationSucceededPleaseLogin));
            }
            else
            {
                view.WriteCode(string.Format("registerForm.proxy().showError('{0}');", result.Message));
            }
            return view;
        }

        public struct SignUpResult
        {
            public bool Success
            {
                get;
                private set;
            }

            public string Message
            {
                get;
                private set;
            }

            public SignUpResult(bool success,string message)
            {
                this.Success = success;
                this.Message = message;
            }

            public static SignUpResult CreateSuccess()
            {
                return new SignUpResult(true, string.Empty);
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual SignUpResult SignUp(DTObject data)
        {
            return new SignUpResult(false, Strings.NotImplementedSignUp);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual bool SignIn(DTObject data)
        {
            data.Transform("username=>flag");
            data.SetValue("photoWidth", 80);
            data.SetValue("photoHeight", 80);
            var handler = ModuleController.GetHandler("login");
            var result = handler.Process(data);
            return result.GetValue<bool>("success");
        }


    }
}
