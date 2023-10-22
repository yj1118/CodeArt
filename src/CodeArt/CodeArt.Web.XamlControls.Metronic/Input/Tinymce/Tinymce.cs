using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;


namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Tinymce.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Tinymce : TextBox
    {
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register<int, Tinymce>("Height", () => { return 500; });

        public int Height
        {
            get
            {
                return (int)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// 上传文件时，提交给request头的数据
        /// </summary>
        public readonly static DependencyProperty DropzoneHeadersProperty = DependencyProperty.Register<UIElementCollection, Tinymce>("DropzoneHeaders", () => { return new UIElementCollection(); });

        public UIElementCollection DropzoneHeaders
        {
            get
            {
                return GetValue(DropzoneHeadersProperty) as UIElementCollection;
            }
            set
            {
                SetValue(DropzoneHeadersProperty, value);
            }
        }

        static Tinymce()
        {

        }

    }
}
