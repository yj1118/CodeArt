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
    [ComponentLoaderFactory(typeof(ManualComponentLoader))]
    [ComponentLoader(typeof(InputUploadLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.UploadTemplate.html,CodeArt.Web.XamlControls.Metronic")]
    public class Upload : Input
    {
        public static DependencyProperty TargetProperty { get; private set; }

        public static DependencyProperty DiskRootIdProperty { get; private set; }

        static Upload()
        {
            var targetMetadata = new PropertyMetadata(() => { return string.Empty; });
            TargetProperty = DependencyProperty.Register<string, Upload>("Target", targetMetadata);

            var diskRootIdMetadata = new PropertyMetadata(() => { return string.Empty; });
            DiskRootIdProperty = DependencyProperty.Register<string, Upload>("DiskRootId", diskRootIdMetadata);
        }

        public string Target
        {
            get
            {
                return GetValue(TargetProperty) as string;
            }
            set
            {
                SetValue(TargetProperty, value);
            }
        }

        /// <summary>
        /// 上传的目标目录的编号
        /// </summary>
        public string DiskRootId
        {
            get
            {
                return GetValue(DiskRootIdProperty) as string;
            }
            set
            {
                SetValue(DiskRootIdProperty, value);
            }
        }

        protected override void OnGotType(ref object baseValue)
        {
            baseValue = "upload";
        }
    }
}