using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ComponentLoader(typeof(DiskLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Disk.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Disk : Control
    {
        public static DependencyProperty RootIdProperty { get; private set; }

        public static DependencyProperty AssetsPathProperty { get; private set; }

        static Disk()
        {
            var rootIdMetadata = new PropertyMetadata(() => { return string.Empty; });
            RootIdProperty = DependencyProperty.Register<string, Disk>("RootId", rootIdMetadata);

            var assetsPathMetadata = new PropertyMetadata(() => { return string.Empty; });
            AssetsPathProperty = DependencyProperty.Register<string, Disk>("AssetsPath", assetsPathMetadata);
        }

        /// <summary>
        /// 根目录的编号
        /// </summary>
        public string RootId
        {
            get
            {
                return GetValue(RootIdProperty) as string;
            }
            set
            {
                SetValue(RootIdProperty, value);
            }
        }

        /// <summary>
        /// 资产路径，该路径一般是连接外网的路径
        /// 固化的
        /// </summary>
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



        public Disk()
        {
        }
    }
}
