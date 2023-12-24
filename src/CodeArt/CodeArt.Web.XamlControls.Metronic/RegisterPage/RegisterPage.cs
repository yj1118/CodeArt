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
using CodeArt.Web.WebPages;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.RegisterPage.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class RegisterPage : CodeArt.Web.WebPages.Xaml.Controls.Page
    {
        public static DependencyProperty LogoProperty = DependencyProperty.Register<string, RegisterPage>("Logo", new PropertyMetadata(() => { return string.Empty; }, OnLogoChanged));

        public static DependencyProperty RegisterButtonTextProperty { get; private set; }

        public static DependencyProperty SuccessUrlProperty { get; private set; }

        public static DependencyProperty RegisterTitleProperty { get; private set; }

        public static DependencyProperty SMSProviderProperty { get; private set; }

        public static DependencyProperty CodeDigitProperty = DependencyProperty.Register<byte, RegisterPage>("CodeDigit", () => {return 1;});

        public static readonly DependencyProperty BackgroundImageProperty = DependencyProperty.Register<string, RegisterPage>("BackgroundImage", () => { return string.Empty; });

        public static readonly DependencyProperty LoginUrlProperty = DependencyProperty.Register<string, RegisterPage>("LoginUrl", () => { return string.Empty; });

        public string LoginUrl
        {
            get
            {
                return GetValue(LoginUrlProperty) as string;
            }
            set
            {
                SetValue(LoginUrlProperty, value);
            }
        }


        static RegisterPage()
        {
            var successUrlMetadata = new PropertyMetadata(() => { return string.Empty; });
            SuccessUrlProperty = DependencyProperty.Register<string, RegisterPage>("SuccessUrl", successUrlMetadata);

            var registerButtonTextMetadata = new PropertyMetadata(() => { return string.Empty; });
            RegisterButtonTextProperty = DependencyProperty.Register<string, RegisterPage>("RegisterButtonText", registerButtonTextMetadata);

            var registerTitleMetadata = new PropertyMetadata(() => { return string.Empty; });
            RegisterTitleProperty = DependencyProperty.Register<string, RegisterPage>("RegisterTitle", registerTitleMetadata);

            var smsProviderMetadata = new PropertyMetadata(() => { return string.Empty; });
            SMSProviderProperty = DependencyProperty.Register<string, RegisterPage>("SMSProvider", smsProviderMetadata);
        }

        private static void OnLogoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as RegisterPage).OnLogoChanged(e);
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
        /// 注册成功后的跳转地址
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

        public string SMSProvider
        {
            get
            {
                return GetValue(SMSProviderProperty) as string;
            }
            set
            {
                SetValue(SMSProviderProperty, value);
            }
        }

        public byte CodeDigit
        {
            get
            {
                return (byte)GetValue(CodeDigitProperty);
            }
            set
            {
                SetValue(CodeDigitProperty, value);
            }
        }

        public RegisterPage()
        {
            this.RegisterScriptAction("Register", Register);
            this.RegisterScriptAction("GetVerificationCode", GetVerificationCode);
        }

        private IScriptView Register(ScriptView view)
        {
            var form = view.GetElement<FormSE>("registerForm");
            var data = form.Data;
            var result = SignUp(data);
            if (result.Success)
            {
                data["ip"] = HttpContext.Current.Request.UserHostAddress;
                if (SignIn(data))
                {
                    view.Redirect(this.SuccessUrl);
                }
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


        private IScriptView GetVerificationCode(ScriptView view)
        {
            var data = view.GetSender().Data;
            data.Provider = SMSProvider;
            data.Digit = CodeDigit;
            var handler = ModuleController.GetHandler("getVerificationCode");
            var result = handler.Process(data);
            return view;
        }


    }
}
