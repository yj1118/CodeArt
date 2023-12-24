using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls
{
    public class PageTurn : FrameworkElement
    {
        public static readonly DependencyProperty StepProperty;

        public static readonly DependencyProperty PageSizeProperty;

        public static readonly DependencyProperty DataCountProperty;

        public static readonly DependencyProperty AnchorProperty;

        static PageTurn()
        {
            var stepMetadata = new PropertyMetadata(() => { return 5; });
            StepProperty = DependencyProperty.Register<int, PageTurn>("Step", stepMetadata);

            var pageSizeMetadata = new PropertyMetadata(() => { return 5; });
            PageSizeProperty = DependencyProperty.Register<int, PageTurn>("PageSize", pageSizeMetadata);

            var dataCountMetadata = new PropertyMetadata(() => { return 0; });
            DataCountProperty = DependencyProperty.Register<int, PageTurn>("DataCount", dataCountMetadata);

            var anchorMetadata = new PropertyMetadata(() => { return string.Empty; });
            AnchorProperty = DependencyProperty.Register<string, PageTurn>("Anchor", anchorMetadata);
        }

        /// <summary>
        /// 步长
        /// </summary>
        public int Step
        {
            get
            {
                return (int)GetValue(StepProperty);
            }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        /// <summary>
        /// 每页显示的数据数量
        /// </summary>
        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        /// <summary>
        /// 总数据数量
        /// </summary>
        public int DataCount
        {
            get
            {
                return (int)GetValue(DataCountProperty);
            }
            set
            {
                SetValue(DataCountProperty, value);
            }
        }

        /// <summary>
        /// 锚标记的对象编号
        /// </summary>
        public string Anchor
        {
            get
            {
                return (string)GetValue(AnchorProperty);
            }
            set
            {
                SetValue(AnchorProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            var request = WebPageContext.Current.Request;
            var step = this.Step;

            var query = WebUtil.ProcessQueryString(request.QueryString);
            var baseUrl = request.Path;
            var pageCount = GetPageCount(this.PageSize, this.DataCount);
            var currentIndex = GetCurrentIndex(query);

            Pagination p = new Pagination(baseUrl, query, pageCount, currentIndex, this.Step, this.Anchor);
            if (pageCount < 2) return;

            StringBuilder html = new StringBuilder();
            html.AppendFormat("<div class=\"{0} pagiation-page\">", this.Class);
            html.AppendLine("<ul class=\"pagination margin-none\">");
            p.Write(html);
            html.AppendLine("</ul>");
            html.Append("</div>");

            brush.Draw(html.ToString(), DrawOrigin.Current);
        }


        private static int GetPageCount(int pageSize, int dataCount)
        {
            int pageCount = dataCount / pageSize;
            if (dataCount % pageSize > 0) pageCount++;
            return pageCount;
        }

        private int GetCurrentIndex(NameValueCollection query)
        {
            return string.IsNullOrEmpty(query["p"]) ? 0 : int.Parse(query["p"]);
        }

        private sealed class Pagination
        {
            public int CurrentIndex
            {
                get;
                private set;
            }

            public int PageCount
            {
                get;
                private set;
            }

            private int _startIndex;

            private int _endIndex;


            private string _baseUrl;
            private string GetUrl(int index)
            {
                _query.Set("p", index.ToString());
                return string.IsNullOrEmpty(_anchor) ? string.Format("{0}?{1}", _baseUrl, _query.ToQueryString()) : string.Format("{0}?{1}#{2}", _baseUrl, _query.ToQueryString(), _anchor);
            }

            private string _anchor;

            private NameValueCollection _query = null;

            public Pagination(string baseUrl, NameValueCollection query, int pageCount, int currentIndex, int step, string anchor)
            {
                _baseUrl = baseUrl;
                _query = query;
                this.PageCount = pageCount;
                this.CurrentIndex = currentIndex;

                _startIndex = currentIndex - step;
                if (_startIndex < 0) _startIndex = 0;

                _endIndex = currentIndex + step;
                if (_endIndex >= pageCount) _endIndex = pageCount - 1;

                _anchor = anchor;
            }

            public void Write(StringBuilder html)
            {
                WritePrev(html);
                Write(html, _startIndex, this.CurrentIndex - 1);

                html.AppendFormat("<li class=\"active\"><a href=\"{0}\">{1}</a></li>", this.GetUrl(this.CurrentIndex), this.CurrentIndex + 1);
                html.AppendLine();

                Write(html, this.CurrentIndex + 1, _endIndex);
                WriteNext(html);
            }

            private void WritePrev(StringBuilder html)
            {
                html.AppendFormat("<li {0}><a href=\"{1}\"><</a></li>",
                                   this.IsFirstPage ? "class=\"disabled\"" : string.Empty,
                                   this.IsFirstPage ? "javascript:;" : this.GetUrl(this.CurrentIndex - 1));
                html.AppendLine();
            }

            private void WriteNext(StringBuilder html)
            {
                html.AppendFormat("<li {0}><a href=\"{1}\">></a></li>",
                this.IsLastPage ? "class=\"disabled\"" : string.Empty,
                this.IsLastPage ? "javascript:;" : this.GetUrl(this.CurrentIndex + 1));
                html.AppendLine();
            }

            private void Write(StringBuilder html, int startIndex, int endIndex)
            {
                int count = endIndex + 1;
                for (var i = startIndex; i < count; i++)
                {
                    html.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", this.GetUrl(i), i + 1);
                    html.AppendLine();
                }
            }


            /// <summary>
            /// 当前处于第一页
            /// </summary>
            /// <returns></returns>
            public bool IsFirstPage
            {
                get
                {
                    return this.CurrentIndex == 0;
                }
            }

            public bool IsLastPage
            {
                get
                {
                    return this.CurrentIndex == this.PageCount - 1;
                }
            }

            public bool IsCurrentPage(int index)
            {
                return this.CurrentIndex == index;
            }


        }


    }
}
