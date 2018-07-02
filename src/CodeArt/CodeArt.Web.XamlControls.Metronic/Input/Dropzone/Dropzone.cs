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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Dropzone.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Dropzone : Input
    {
        /// <summary>
        /// 最大上传文件的个数，0代表不限制
        /// </summary>
        public static readonly DependencyProperty MaxFilesProperty = DependencyProperty.Register<int, Dropzone>("MaxFiles", () => { return 0; });

        public int MaxFiles
        {
            get
            {
                return (int)GetValue(MaxFilesProperty);
            }
            set
            {
                SetValue(MaxFilesProperty, value);
            }
        }

        public static readonly DependencyProperty MaxFileSizeProperty = DependencyProperty.Register<int, Dropzone>("MaxFileSize", () => { return 1; });
        /// <summary>
        ///  文件最大的体积，单位M，默认1M
        /// </summary>
        public int MaxFileSize
        {
            get
            {
                return (int)GetValue(MaxFileSizeProperty);
            }
            set
            {
                SetValue(MaxFileSizeProperty, value);
            }
        }

        public static readonly DependencyProperty AcceptedFilesProperty = DependencyProperty.Register<string, Dropzone>("AcceptedFiles", () => { return string.Empty; });
        /// <summary>
        ///  默认实现accept根据此列表检查文件的MIME类型或扩展名。这是一个以逗号分隔的MIME类型或文件扩展名列表。
        ///  例如。： image/*,application/pdf,.psd
        ///  如果Dropzone是clickable这个选项，它也将用作 accept 隐藏文件输入的参数。
        /// </summary>
        public string AcceptedFiles
        {
            get
            {
                return GetValue(AcceptedFilesProperty) as string;
            }
            set
            {
                SetValue(AcceptedFilesProperty, value);
            }
        }

        public static readonly DependencyProperty UploadUrlProperty = DependencyProperty.Register<string, Dropzone>("UploadUrl", () => { return string.Empty; });
        /// <summary>
        /// 上传的目标地址
        /// </summary>
        public string UploadUrl
        {
            get
            {
                return GetValue(UploadUrlProperty) as string;
            }
            set
            {
                SetValue(UploadUrlProperty, value);
            }
        }

        public static readonly DependencyProperty LoadThumbnailUrlProperty = DependencyProperty.Register<string, Dropzone>("LoadThumbnailUrl", () => { return string.Empty; });
        /// <summary>
        /// 加载缩略图的url
        /// </summary>
        public string LoadThumbnailUrl
        {
            get
            {
                return GetValue(LoadThumbnailUrlProperty) as string;
            }
            set
            {
                SetValue(LoadThumbnailUrlProperty, value);
            }
        }

        /// <summary>
        /// 上传文件时，提交给request头的数据
        /// </summary>
        public readonly static DependencyProperty HeadersProperty = DependencyProperty.Register<UIElementCollection, Dropzone>("Headers", () => { return new UIElementCollection(); });

        public UIElementCollection Headers
        {
            get
            {
                return GetValue(HeadersProperty) as UIElementCollection;
            }
            set
            {
                SetValue(HeadersProperty, value);
            }
        }
    }
}
