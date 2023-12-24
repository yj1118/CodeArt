using CodeArt.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 链接形式的代码，这主要是外链javascript,css等资源用
    /// </summary>
    public class LinkCode : CodeAsset
    {
        /// <summary>
        /// 如果通过此key能够找到外部链接，那么使用外部链接，否则使用本地链接
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 这是css链接需要用到的参数
        /// </summary>
        public string Media
        {
            get;
            set;
        }

        public string Src
        {
            get;
            set;
        }

        protected string GetSrc()
        {
            if (!string.IsNullOrEmpty(this.Src)) return this.Src;
            if (string.IsNullOrEmpty(this.Key)) throw new XamlException("没有为LinkCode设置Key");
            return GetExternal() ?? GetInternal();
        }

        private string GetExternal()
        {
            var app = Application.Current;
            var external = app.FindLink(this.Key);
            return external != null ? external.Src : null;
        }

        private string GetInternal()
        {
            var template = this.BelongTemplate;
            var file = template == null ? null : template.GetFile(this.Key);
            if (file == null)
                throw new XamlException("没有找到" + this.Key + "对应的内部文件");
            return file.VirtualPath;
        }


        private string _cacheCode;


        protected override string GetCode()
        {
            if(_cacheCode == null)
            {
                lock(this)
                {
                    if(_cacheCode == null)
                    {
                        using (var temp = StringPool.Borrow())
                        {
                            var code = temp.Item;
                            var path = GetSrc();
                            var version = Application.Current.AssetVersion;
                            if (path.EndsWith(".css"))
                            {
                                code.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}?v={1}\" ", path, version);
                                if (!string.IsNullOrEmpty(this.Media))
                                {
                                    code.AppendFormat("media=\"{0}\" ", this.Media);
                                }
                                code.Append("/>");
                            }
                            else if (path.EndsWith(".js"))
                                code.AppendFormat("<script type=\"text/javascript\" src=\"{0}?v={1}\" charset=\"utf-8\"></script>", path, version);
                            else
                                throw new XamlException("无法识别的附件链接" + path);
                            _cacheCode = code.ToString();
                        }
                    }
                }
            }
            return _cacheCode;
        }

        protected override void Draw(PageBrush brush)
        {
            if (brush.Contains(this.GetCode())) return;
            base.Draw(brush);

            if (this.Origin == DrawOrigin.Header || this.Origin == DrawOrigin.Bottom)
                brush.DrawLine(this.Origin);

        }

    }
}
