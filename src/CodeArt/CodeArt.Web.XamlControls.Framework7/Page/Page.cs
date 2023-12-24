using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;

using CodeArt.Web.WebPages;

namespace CodeArt.Web.XamlControls.Framework7
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Framework7.Page.Template.html,CodeArt.Web.XamlControls.Framework7")]
    public class Page : CodeArt.Web.WebPages.Xaml.Controls.Page
    {
        public static readonly DependencyProperty LogoProperty = DependencyProperty.Register<string, Page>("Logo", () => { return string.Empty; });

        public static DependencyProperty AssetsPathProperty { get; private set; }

       
        public static readonly DependencyProperty LogoStyleProperty = DependencyProperty.Register<string, Page>("LogoStyle", () => { return string.Empty; });


        static Page()
        {
            var assetsPathMetadata = new PropertyMetadata(() => { return string.Empty; });
            AssetsPathProperty = DependencyProperty.Register<string, Page>("AssetsPath", assetsPathMetadata);
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

        public static DependencyProperty PrincipalIdProperty = DependencyProperty.Register<long, Page>("PrincipalId", new PropertyMetadata(() => { return 0L; }));

        /// <summary>
        /// 
        /// </summary>
        public long PrincipalId
        {
            get
            {
                return (long)GetValue(PrincipalIdProperty);
            }
            set
            {
                SetValue(PrincipalIdProperty, value);
            }
        }

        public Page()
        {
            this.DataContext = DTObject.Empty;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, object e)
        {
            if (!this.IsScriptCallback)
            {
                this.DataContext = GetData();
            }
            this.PrincipalId = Util.GetPrincipalId(this);
        }

        /// <summary>
        /// 获取页面数据
        /// </summary>
        /// <returns></returns>
        protected virtual DTObject GetData()
        {
            return DTObject.Empty;
        }

    }
}
