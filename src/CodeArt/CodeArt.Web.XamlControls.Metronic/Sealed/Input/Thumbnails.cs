using CodeArt.Web.WebPages.Xaml.Sealed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.DTO;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    public class Thumbnails : SealedControl
    {
        public static DependencyProperty DiskRootIdProperty { get; private set; }

        static Thumbnails()
        {
            var diskRootIdMetadata = new PropertyMetadata(() => { return string.Empty; }, OnDiskRootIdChanged);
            DiskRootIdProperty = DependencyProperty.Register<string, Thumbnails>("DiskRootId", diskRootIdMetadata);
        }


        private static void OnDiskRootIdChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var target = obj as Thumbnails;
            target.OnDiskRootIdChanged(e);
        }

        protected virtual void OnDiskRootIdChanged(DependencyPropertyChangedEventArgs e)
        {
            var upload = this.GetChild("upload") as Upload;
            upload.DiskRootId = e.NewValue as string;

            var cropper = this.GetChild("cropper") as Cropper;
            cropper.DiskRootId = e.NewValue as string;
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

        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            List<CodeAsset> assets = new List<CodeAsset>();
            assets.Add(new LinkCode() { ExternalKey = "bootstrap.holder.js", Origin = DrawOrigin.Bottom });
            assets.Add(new LinkCode() { ExternalKey = "codeArt.component.input.thumbnails.js", Origin = DrawOrigin.Bottom });
            return assets.ToArray();
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            node.SetAttributeValue("type", "thumbnails");
            InputThumbnailsPainter.Instance.FillHtml(this, node, brush);
        }
    }
}
