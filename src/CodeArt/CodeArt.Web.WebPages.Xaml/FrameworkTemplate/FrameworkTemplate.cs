using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [ContentProperty("Template")]
    [ComponentLoader(typeof(FrameworkTemplateLoader))]
    public class FrameworkTemplate : DependencyObject, IFrameworkTemplate, IHtmlElementCore
    {
        #region 依赖属性

        public static DependencyProperty TemplateProperty { get; private set; }

        public static DependencyProperty AttributesProperty { get; private set; }

        public static DependencyProperty DirectoriesProperty { get; private set; }

        public static DependencyProperty TemplateParentProperty { get; private set; }

        static FrameworkTemplate()
        {
            var templateMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            TemplateProperty = DependencyProperty.Register<UIElementCollection, FrameworkTemplate>("Template", templateMetadata);

            var attributesMetadata = new PropertyMetadata(() => { return new CustomAttributeCollection(); });
            AttributesProperty = DependencyProperty.Register<CustomAttributeCollection, FrameworkTemplate>("Attributes", attributesMetadata);

            DirectoriesProperty = DependencyProperty.Register<DependencyCollection, FrameworkTemplate>("Directories", () => { return new DependencyCollection(); });

            var templateParentMetadata = new PropertyMetadata(() => { return null; }, OnTemplateParentChanged);
            TemplateParentProperty = DependencyProperty.Register<object, FrameworkTemplate>("TemplateParent", templateParentMetadata);
        }

        private static void OnTemplateParentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var target = obj as FrameworkTemplate;
            target.OnTemplateParentChanged(e);
        }

        protected virtual void OnTemplateParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TemplateParentChanged != null)
                this.TemplateParentChanged(this, e);
        }

        public event DependencyPropertyChangedEventHandler TemplateParentChanged;


        #endregion

        [IgnoreBlank(true)]
        public UIElementCollection Template
        {
            get
            {
                return GetValue(TemplateProperty) as UIElementCollection;
            }
            set
            {
                SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// 在渲染期间应用该模板的对象
        /// 一个模板不能同时应用于多个对象
        /// </summary>
        public object TemplateParent
        {
            get
            {
                return GetValue(TemplateParentProperty);
            }
            set
            {
                SetValue(TemplateParentProperty, value);
            }
        }

        /// <summary>
        /// 渲染
        /// </summary>
        /// <param name="templateParent"></param>
        /// <param name="brush"></param>
        public void Render(PageBrush brush)
        {
            var templateParent = this.TemplateParent;
            if (templateParent == null) throw new XamlException("没有设置模板的应用对象，无法渲染");
            RenderContext.Current.PushObject(this);
            Render(templateParent, brush);
            RenderContext.Current.PopObject();
        }

        protected virtual void Render(object templateParent, PageBrush brush) { }

        public DependencyObject GetChild(string childName)
        {
            return this.Template.GetChild(childName);
        }

        /// <summary>
        /// 自定义属性
        /// </summary>
        public CustomAttributeCollection Attributes
        {
            get
            {
                return GetValue(AttributesProperty) as CustomAttributeCollection;
            }
            set
            {
                SetValue(AttributesProperty, value);
            }
        }

        #region 模板引用的资产目录

        public DependencyCollection Directories
        {
            get
            {
                return GetValue(DirectoriesProperty) as DependencyCollection;
            }
            set
            {
                SetValue(DirectoriesProperty, value);
            }
        }

        /// <summary>
        /// 获取该模板中的资产文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AssetFile GetFile(string key)
        {
            if (this.Directories == null) return null;
            foreach(AssetDirectory directory in this.Directories)
            {
                var file = directory.GetFile(key);
                if (file != null) return file;
            }
            return null;
        }

        #endregion

        public override void OnInit()
        {
            //为了节省内存，固化attributes，这样其余线程访问时，会加载该对象作为本地值，而不是重新new新的对象
            if (this.Attributes == null) this.Attributes = new CustomAttributeCollection();
            base.OnInit();
        }

        public override void OnLoad()
        {
            this.Template.OnLoad();
            base.OnLoad();
        }

        /// <summary>
        /// 模板的代码
        /// </summary>
        public string TemplateCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// 克隆模板
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            if (string.IsNullOrEmpty(this.TemplateCode)) throw new XamlException("没有定义模板代码，不能克隆模板");
            return XamlReader.ReadComponent(this.TemplateCode);
        }


#if (DEBUG)

        private static Stack<string> _trackCodes = new Stack<string>();

        internal static void PushTrackCode(string code)
        {
            _trackCodes.Push(code);
        }

        internal static void PopTrackCode()
        {
            _trackCodes.Pop();
        }

        /// <summary>
        /// 跟踪当前正在分析的模板的代码
        /// </summary>
        internal static string TrackCode
        {
            get
            {
                return _trackCodes.Count == 0 ? null : _trackCodes.First();
            }
        }
#endif

    }
}