using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Sealed
{
    /// <summary>
    /// 密封的组件，该组件不依赖模板的输出，而是通过解析dom代码自行绘制输出流
    /// 该类主要用于快速构建框架之前的做好的组件或者组件本身确实不需要任何扩展的时候使用，
    /// 建议尽量不使用该类
    /// </summary>
    [ComponentLoader(typeof(SealedControlLoader))]
    public abstract class SealedControl : FrameworkElement
    {
        static SealedControl()
        {
        }

        public SealedControl()
        {
            this.Elements = new InternalElements();
        }

        /// <summary>
        /// 源代码
        /// </summary>
        public string SourceCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// 缓存的画刷
        /// </summary>
        public PageBrush CacheBrush
        {
            get;
            private set;
        }

        /// <summary>
        /// 该密封组件内部使用到的元素
        /// </summary>
        public InternalElements Elements
        {
            get;
            private set;
        }

        public override void LoadPinned()
        {
            base.LoadPinned();
            this.Elements.LoadPinned();
        }

        public override void OnInit()
        {
            var node = GetNode();
            CreateBrush(node); //在此处创建画刷不是为了得到画刷，而是为了执行分析代码，收集相关数据,建立控件连接
            this.OnInit(node);
            base.OnInit();
        }

        protected virtual void OnInit(HtmlNode node)
        {

        }

        
        protected override void Draw(PageBrush brush)
        {
            InitCacheBrush();
            brush.Combine(this.CacheBrush);
        }

        protected void InitCacheBrush()
        {
            if (this.CacheBrush == null)
            {
                lock (this)
                {
                    if (this.CacheBrush == null)
                    {
                        this.Elements.Reset();
                        var node = GetNode();
                        this.CacheBrush = CreateBrush(node);
                    }
                }
            }
        }

        private PageBrush CreateBrush(HtmlNode node)
        {
            var brush = new PageBrush();

            FillCode(node, brush);

            var assets = GetCodeAssets(node);
            if (assets != null)
            {
                foreach (var asset in assets) asset.Render(brush);
            }
            return brush;
        }


        protected HtmlNode GetNode()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(this.SourceCode);
            return doc.DocumentNode.ChildElements().FirstOrDefault();
        }

        /// <summary>
        /// 填充输出代码
        /// </summary>
        /// <param name="node"></param>
        /// <param name="brush"></param>
        /// <returns></returns>
        protected abstract void FillCode(HtmlNode node, PageBrush brush);

        /// <summary>
        /// 获取代码形式的资产
        /// </summary>
        /// <returns></returns>
        protected abstract CodeAsset[] GetCodeAssets(HtmlNode node);

        public override DependencyObject GetChild(string childName)
        {
            return this.Elements.GetChild(childName) ?? base.GetChild(childName);
        }

    }
}
