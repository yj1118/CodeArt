using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
   [TemplateCodeFactory("Template",typeof(PageTemplateCodeFactory))]
    public class Page : CodeArt.Web.WebPages.Xaml.Controls.Page
    {
        public static readonly DependencyProperty LogoProperty = DependencyProperty.Register<string, Page>("Logo", () => { return string.Empty; });

        public static readonly DependencyProperty PrincipalNameProperty = DependencyProperty.Register<string, Page>("PrincipalName", () => { return string.Empty; });

        public static readonly DependencyProperty PrincipalEmailProperty = DependencyProperty.Register<string, Page>("PrincipalEmail", () => { return string.Empty; });

        public static readonly DependencyProperty PrincipalPhotoProperty = DependencyProperty.Register<string, Page>("PrincipalPhoto", () => { return string.Empty; });

        public static readonly DependencyProperty LogoutUrlProperty = DependencyProperty.Register<string, Page>("LogoutUrl", () => { return "/logout.htm"; });

        public static readonly DependencyProperty HomeUrlProperty = DependencyProperty.Register<string, Page>("HomeUrl", () => { return "/index.htm"; });


        public static DependencyProperty AssetsPathProperty { get; private set; }

        public static DependencyProperty RootMenuProperty { get; private set; }

        public static DependencyProperty IsLocalMenuProperty { get; private set; }


        public static DependencyProperty WorkTitleProperty { get; private set; }

        public static DependencyProperty WorkDescriptionProperty { get; private set; }

        public static DependencyProperty ShowWorkHeaderProperty { get; private set; }

        public static DependencyProperty MenuRedirectProperty { get; private set; }

        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register<string, Page>("Theme", () => { return "default"; });

        public static readonly DependencyProperty LogoStyleProperty = DependencyProperty.Register<string, Page>("LogoStyle", () => { return string.Empty; });

        static Page()
        {
            var assetsPathMetadata = new PropertyMetadata(() => { return string.Empty; });
            AssetsPathProperty = DependencyProperty.Register<string, Page>("AssetsPath", assetsPathMetadata);

            var rootMenuMetadata = new PropertyMetadata(() => { return string.Empty; });
            RootMenuProperty = DependencyProperty.Register<string, Page>("RootMenu", rootMenuMetadata);

            var isLocalMenuMetadata = new PropertyMetadata(() => { return false; });
            IsLocalMenuProperty = DependencyProperty.Register<bool, Page>("IsLocalMenu", isLocalMenuMetadata);

            var workTitleMetadata = new PropertyMetadata(() => { return string.Empty; });
            WorkTitleProperty = DependencyProperty.Register<string, Page>("WorkTitle", workTitleMetadata);

            var workDescriptionMetadata = new PropertyMetadata(() => { return string.Empty; });
            WorkDescriptionProperty = DependencyProperty.Register<string, Page>("WorkDescription", workDescriptionMetadata);

            var showWorkHeaderMetadata = new PropertyMetadata(() => { return true; });
            ShowWorkHeaderProperty = DependencyProperty.Register<bool, Page>("ShowWorkHeader", showWorkHeaderMetadata);

            var menuRedirectMetadata = new PropertyMetadata(() => { return string.Empty;});
            MenuRedirectProperty = DependencyProperty.Register<string, Page>("MenuRedirect", menuRedirectMetadata);

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

        public string PrincipalEmail
        {
            get
            {
                return GetValue(PrincipalEmailProperty) as string;
            }
            set
            {
                SetValue(PrincipalEmailProperty, value);
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

        public string LogoutUrl
        {
            get
            {
                return GetValue(LogoutUrlProperty) as string;
            }
            set
            {
                SetValue(LogoutUrlProperty, value);
            }
        }

        public string HomeUrl
        {
            get
            {
                return GetValue(HomeUrlProperty) as string;
            }
            set
            {
                SetValue(HomeUrlProperty, value);
            }
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

      
        public virtual string GetMenuCode()
        {
            DTObject para = DTObject.Create();
            para.SetValue("markedCode", this.RootMenu);
            para.SetValue("isLocal", this.IsLocalMenu);

            var handler = ModuleController.GetHandler("menu.show");
            var result = handler.Process(para);
            return result.GetValue<string>("menuCode");
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

        //private static void OnShowWorkHeaderChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //{
        //    (obj as Page).OnShowWorkHeaderChanged(e);
        //}

        //protected virtual void OnShowWorkHeaderChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    UpdateWorkHeaderVisibility();
        //}

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
    }
}
