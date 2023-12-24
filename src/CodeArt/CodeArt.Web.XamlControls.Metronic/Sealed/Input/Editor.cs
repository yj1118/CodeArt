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
    public class Editor : SealedControl
    {
        public static DependencyProperty DiskRootIdProperty { get; private set; }



        static Editor()
        {
            var diskRootIdMetadata = new PropertyMetadata(() => { return string.Empty; }, OnDiskRootIdChanged);
            DiskRootIdProperty = DependencyProperty.Register<string, Editor>("DiskRootId", diskRootIdMetadata);
        }


        private static void OnDiskRootIdChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var editor = obj as Editor;
            editor.OnDiskRootIdChanged(e);
        }

        protected virtual void OnDiskRootIdChanged(DependencyPropertyChangedEventArgs e)
        {
            var disk = this.GetChild("disk") as Disk;
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


        public Editor()
        {
            this.Inited += OnInited;
        }

        private void OnInited(object sender, object e)
        {
            this.RegisterScriptAction("OnEditorInited", OnEditorInited);
        }

        private IScriptView OnEditorInited(ScriptView view)
        {
            if (this.EditorInited == null) return view;
            var sender = view.GetSender<EditorSE>();
            return this.EditorInited(view, sender);
        }

        /// <summary>
        /// 当客户端的文本编辑器初始化完毕时触发
        /// </summary>
        public Func<ScriptView, EditorSE, ScriptView> EditorInited = null;

        protected override CodeAsset[] GetCodeAssets(HtmlNode node)
        {
            List<CodeAsset> assets = new List<CodeAsset>();
            assets.Add(new LinkCode() { ExternalKey = "tinymce:core.js", Origin = DrawOrigin.Bottom });
            assets.Add(new LinkCode() { ExternalKey = "tinymce:plugin.metronic.disk.js", Origin = DrawOrigin.Bottom });
            assets.Add(new LinkCode() { ExternalKey = "tinymce:codeArt.js", Origin = DrawOrigin.Bottom });
            return assets.ToArray();
        }

        protected override void FillCode(HtmlNode node, PageBrush brush)
        {
            node.SetAttributeValue("type", "editor");
            InputEditorPainter.Instance.FillHtml(this, node, brush);
        }
    }
}
