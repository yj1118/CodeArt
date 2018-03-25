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
    [ComponentLoader(typeof(CropperLoader))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Cropper.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Cropper : Control
    {
        public Cropper()
        {
        }

        public static DependencyProperty DiskRootIdProperty { get; private set; }

  
        static Cropper()
        {
            var diskRootIdMetadata = new PropertyMetadata(() => { return string.Empty; }, OnDiskRootIdChanged);
            DiskRootIdProperty = DependencyProperty.Register<string, Cropper>("DiskRootId", diskRootIdMetadata);
        }


        private static void OnDiskRootIdChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var cropper = obj as Cropper;
            cropper.OnDiskRootIdChanged(e);
        }

        protected virtual void OnDiskRootIdChanged(DependencyPropertyChangedEventArgs e)
        {
            var disk = this.GetTemplateChild("disk") as Disk;
            disk.RootId = e.NewValue as string;
        }

        /// <summary>
        /// 根目录的编号
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

    }
}
