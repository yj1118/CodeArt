using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
   [TemplateCodeFactoryAttribute("Template",typeof(AdminPageTemplateCodeFactory))]
    public class AdminPage : CodeArt.Web.WebPages.Xaml.Controls.Page
    {
        public static DependencyProperty LogoProperty { get; private set; }

        public static DependencyProperty PrincipalNameProperty { get; private set; }

        public static DependencyProperty PrincipalPhotoProperty { get; private set; }

        public static DependencyProperty AssetsPathProperty { get; private set; }

        public static DependencyProperty RootMenuProperty { get; private set; }

        public static DependencyProperty IsLocalMenuProperty { get; private set; }

        public static DependencyAction GetMenuCodeAction { get; private set; }

        public static DependencyProperty WorkTitleProperty { get; private set; }

        public static DependencyProperty WorkDescriptionProperty { get; private set; }

        public static DependencyProperty ShowWorkHeaderProperty { get; private set; }

        public static DependencyProperty MenuRedirectProperty { get; private set; }

        public static DependencyProperty ThemeProperty { get; private set; }

        public static DependencyProperty LogoStyleProperty { get; private set; }

        static AdminPage()
        {
            var logoMetadata = new PropertyMetadata(() => { return string.Empty; }, OnLogoChanged);
            LogoProperty = DependencyProperty.Register<string, AdminPage>("Logo", logoMetadata);

            var principalPhotoMetadata = new PropertyMetadata(() => { return string.Empty; }, OnPrincipalPhotoChanged);
            PrincipalPhotoProperty = DependencyProperty.Register<string, AdminPage>("PrincipalPhoto", principalPhotoMetadata);

            var principalNameMetadata = new PropertyMetadata(() => { return string.Empty; });
            PrincipalNameProperty = DependencyProperty.Register<string, AdminPage>("PrincipalName", principalNameMetadata);

            var assetsPathMetadata = new PropertyMetadata(() => { return string.Empty; });
            AssetsPathProperty = DependencyProperty.Register<string, AdminPage>("AssetsPath", assetsPathMetadata);

            var rootMenuMetadata = new PropertyMetadata(() => { return string.Empty; });
            RootMenuProperty = DependencyProperty.Register<string, AdminPage>("RootMenu", rootMenuMetadata);

            var isLocalMenuMetadata = new PropertyMetadata(() => { return false; });
            IsLocalMenuProperty = DependencyProperty.Register<bool, AdminPage>("IsLocalMenu", isLocalMenuMetadata);

            var getMenuCodeMetadata = new ActionMetadata(GetMenuCodeActionProduce);
            GetMenuCodeAction = DependencyAction.Register<AdminPage>("GetMenuCode", false, getMenuCodeMetadata);

            var workTitleMetadata = new PropertyMetadata(() => { return string.Empty; });
            WorkTitleProperty = DependencyProperty.Register<string, AdminPage>("WorkTitle", workTitleMetadata);

            var workDescriptionMetadata = new PropertyMetadata(() => { return string.Empty; });
            WorkDescriptionProperty = DependencyProperty.Register<string, AdminPage>("WorkDescription", workDescriptionMetadata);

            var showWorkHeaderMetadata = new PropertyMetadata(() => { return true; }, OnShowWorkHeaderChanged);
            ShowWorkHeaderProperty = DependencyProperty.Register<bool, AdminPage>("ShowWorkHeader", showWorkHeaderMetadata);

            var menuRedirectMetadata = new PropertyMetadata(() => { return string.Empty;});
            MenuRedirectProperty = DependencyProperty.Register<string, AdminPage>("MenuRedirect", menuRedirectMetadata);

            var themeMetadata = new PropertyMetadata(() => { return "darkblue"; });
            ThemeProperty = DependencyProperty.Register<string, AdminPage>("Theme", themeMetadata);

            var logoStyleMetadata = new PropertyMetadata(() => { return string.Empty; });
            LogoProperty = DependencyProperty.Register<string, AdminPage>("LogoStyle", logoStyleMetadata);
        }

        private static void OnLogoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as AdminPage).OnLogoChanged(e);
        }

        protected virtual void OnLogoChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LoadContext.IsLoading) return;
            UpdateLogoVisibility();
        }


        /// <summary>
        /// 
        /// </summary>
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

        public string LogoStyle
        {
            get
            {
                return GetValue(LogoStyleProperty) as string;
            }
            set
            {
                SetValue(LogoStyleProperty, value);
            }
        }

        public string Theme
        {
            get
            {
                return GetValue(ThemeProperty) as string;
            }
            set
            {
                SetValue(ThemeProperty, value);
            }
        }

        public string PrincipalName
        {
            get
            {
                return GetValue(PrincipalNameProperty) as string;
            }
            set
            {
                SetValue(PrincipalNameProperty, value);
            }
        }

        public string PrincipalPhoto
        {
            get
            {
                return GetValue(PrincipalPhotoProperty) as string;
            }
            set
            {
                SetValue(PrincipalPhotoProperty, value);
            }
        }

        private static void OnPrincipalPhotoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as AdminPage).OnPrincipalPhotoChanged(e);
        }

        protected virtual void OnPrincipalPhotoChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LoadContext.IsLoading) return;
            UpdatePhotoVisibility();
        }

       
        /// <summary>
        /// 更新照片的显示状态
        /// </summary>
        private void UpdatePhotoVisibility()
        {
            if (this.Template == null) return;
            var photo = this.PrincipalPhoto;
            var img = this.Template.GetChild("principalPhoto") as UIElement;
            img.Visibility = string.IsNullOrEmpty(photo) ? Visibility.Collapsed : Visibility.Visible;
        }

        public string RootMenu
        {
            get
            {
                return GetValue(RootMenuProperty) as string;
            }
            set
            {
                SetValue(RootMenuProperty, value);
            }
        }


        public string AssetsPath
        {
            get
            {
                return GetValue(AssetsPathProperty) as string;
            }
            set
            {
                SetValue(AssetsPathProperty, value);
            }
        }

        private static object GetMenuCodeActionProduce(DependencyObject obj, object[] args)
        {
            return ActionEngine.Execute(args, (obj as AdminPage).GetMenuCodeActionProduce);
        }

        protected virtual string GetMenuCodeActionProduce()
        {
            DTObject para = DTObject.Create();
            para.SetValue("markedCode", this.RootMenu);
            para.SetValue("isLocal", this.IsLocalMenu);

            var handler = ModuleController.GetHandler("menu.show");
            var result = handler.Process(para);
            return result.GetValue<string>("menuCode");
        }

        public string GetMenuCode()
        {
            return CallAction(GetMenuCodeAction) as string;
        }

        /// <summary>
        /// 工作区标题
        /// </summary>
        public string WorkTitle
        {
            get
            {
                return GetValue(WorkTitleProperty) as string;
            }
            set
            {
                SetValue(WorkTitleProperty, value);
            }
        }

        /// <summary>
        /// 工作区描述
        /// </summary>
        public string WorkDescription
        {
            get
            {
                return GetValue(WorkDescriptionProperty) as string;
            }
            set
            {
                SetValue(WorkDescriptionProperty, value);
            }
        }

        public bool ShowWorkHeader
        {
            get
            {
                return (bool)GetValue(ShowWorkHeaderProperty);
            }
            set
            {
                SetValue(ShowWorkHeaderProperty, value);
            }
        }

        public bool IsLocalMenu
        {
            get
            {
                return (bool)GetValue(IsLocalMenuProperty);
            }
            set
            {
                SetValue(IsLocalMenuProperty, value);
            }
        }

        private static void OnShowWorkHeaderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as AdminPage).OnShowWorkHeaderChanged(e);
        }

        protected virtual void OnShowWorkHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateWorkHeaderVisibility();
        }

        public string MenuRedirect
        {
            get
            {
                return GetValue(MenuRedirectProperty) as string;
            }
            set
            {
                SetValue(MenuRedirectProperty, value);
            }
        }

        private void UpdateWorkHeaderVisibility()
        {
            if (this.Template == null) return;
            var show = this.ShowWorkHeader;
            var pageTitle = this.GetTemplateChild("pageTitle") as UIElement;
            var pageBar = this.GetTemplateChild("pageBar") as UIElement;
            pageTitle.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            pageBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }


        public override void OnLoad()
        {
            UpdatePhotoVisibility();
            UpdateWorkHeaderVisibility();
            UpdateLogoVisibility();
            base.OnLoad();
        }
    }
}
