using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;

using CodeArt.Web.WebPages.Xaml.Markup;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    public class PageBrush
    {
        private PageBrushParagraphCode _header = new PageBrushParagraphCode(true);
        private PageBrushParagraphCode _bottom = new PageBrushParagraphCode(true);
        private PageBrushParagraphCode _current = new PageBrushParagraphCode(false);

        public PageBrush()
        {
        }

        public bool Contains(string code)
        {
            return _header.Contains(code) || _bottom.Contains(code) || _current.Contains(code);
        }

        /// <summary>
        /// 绘制代码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="origin"></param>
        public void Draw(string content, DrawOrigin origin = DrawOrigin.Current)
        {
            switch (origin)
            {
                case DrawOrigin.Header: _header.Append(content); break;
                case DrawOrigin.Bottom: _bottom.Append(content); break;
                case DrawOrigin.Current: _current.Append(content); break;
            }
        }

        public void DrawLine(string content, DrawOrigin origin = DrawOrigin.Current)
        {
            switch (origin)
            {
                case DrawOrigin.Header: _header.AppendLine(content); break;
                case DrawOrigin.Bottom: _bottom.AppendLine(content); break;
                case DrawOrigin.Current: _current.AppendLine(content); break;
            }
        }

        public void DrawFormat(string format,params object[] args)
        {
            this.Draw(string.Format(format, args));
        }

        public void DrawLine(DrawOrigin origin = DrawOrigin.Current)
        {
            DrawLine(string.Empty, origin);
        }

        /// <summary>
        /// 获取页面代码
        /// </summary>
        /// <param name="onlyMain">是否仅输出主体代码，如果该选项为true，那么当代码段没有body节点时，是不会输出script,style的代码的</param>
        /// <returns></returns>
        public string GetCode(bool onlyMain = true)
        {
            var code = _current.ToString();
            code = InsertHeader(code, onlyMain);
            code = InserBottom(code, onlyMain);
            return code;
        }

        //private string InsertHeader(string mainCode)
        //{
        //    var code = _header.ToString();
        //    if (string.IsNullOrEmpty(code)) return mainCode;

        //    var pos = mainCode.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
        //    if (pos == -1)
        //    {
        //        //在根画刷中插入代码
        //        var rootBrush = RenderContext.Current.RootBrush;
        //        rootBrush.Draw(code, DrawOrigin.Header);
        //        //return mainCode.Insert(0, code);
        //    }
        //    return mainCode.Insert(pos, code);
        //}

        //private string InserBottom(string mainCode)
        //{
        //    var code = _bottom.ToString();
        //    if (string.IsNullOrEmpty(code)) return mainCode;

        //    var pos = mainCode.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);
        //    if (pos == -1)
        //    {
        //        //在根画刷中插入代码
        //        var rootBrush = RenderContext.Current.RootBrush;
        //        rootBrush.Draw(code, DrawOrigin.Bottom);
        //        //return mainCode.Insert(mainCode.Length, code);
        //    }
        //    return mainCode.Insert(pos, code);
        //}

        private string InsertHeader(string mainCode,bool onlyMain)
        {
            var code = _header.ToString();
            if (string.IsNullOrEmpty(code)) return mainCode;

            var pos = mainCode.IndexOf("</head>", StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
            {
                //在根画刷中插入代码
                if (onlyMain)
                {
                    var rootBrush = RenderContext.Current.RootBrush;
                    rootBrush.Draw(code, DrawOrigin.Header);
                    return mainCode;
                }
                else
                    return mainCode.Insert(0, code);
            }
            return mainCode.Insert(pos, code);
        }

        private string InserBottom(string mainCode, bool onlyMain)
        {
            var code = _bottom.ToString();
            if (string.IsNullOrEmpty(code)) return mainCode;

            var pos = mainCode.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
            {
                if (onlyMain)
                {
                    //在根画刷中插入代码
                    var rootBrush = RenderContext.Current.RootBrush;
                    rootBrush.Draw(code, DrawOrigin.Bottom);
                    return mainCode;
                }
                else
                    return mainCode.Insert(mainCode.Length, code);
            }
            return mainCode.Insert(pos, code);
        }

        /// <summary>
        /// 回退空格、换行符等
        /// </summary>
        /// <returns></returns>
        public void Backspace(DrawOrigin origin = DrawOrigin.Current)
        {
            switch (origin)
            {
                case DrawOrigin.Header: _header.Backspace(); break;
                case DrawOrigin.Bottom: _bottom.Backspace(); break;
                case DrawOrigin.Current: _current.Backspace(); break;
            }
        }

        /// <summary>
        /// 回退字符,如果字符中含有空白符会自动过滤掉而且不参与过滤字数的统计
        /// </summary>
        /// <param name="count">需要回退的字符个数</param>
        /// <param name="origin"></param>
        public void Backspace(int count,DrawOrigin origin = DrawOrigin.Current)
        {
            switch (origin)
            {
                case DrawOrigin.Header: _header.Backspace(count); break;
                case DrawOrigin.Bottom: _bottom.Backspace(count); break;
                case DrawOrigin.Current: _current.Backspace(count); break;
            }
        }

        public void DrawScriptInit(IScriptView view)
        {
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.Append("<script>");
                code.AppendFormat("{0}", view.GetScriptCode());
                code.Append("</script>");
                this.DrawLine(code.ToString(), DrawOrigin.Bottom);
            }
        }

        /// <summary>
        /// 绘制脚本回调
        /// </summary>
        public void DrawScriptCallback(IScriptView view)
        {
            using (var temp = StringPool.Borrow())
            {
                StringBuilder code = temp.Item;
                code.Append("<script>$(document).ready(function () { ");
                code.AppendFormat("$$view.callback({0});", view.GetDataCode());
                code.Append(" });</script>");
                this.DrawLine(code.ToString(), DrawOrigin.Bottom);
            }
        }

        


        /// <summary>
        /// 清空刷子
        /// </summary>
        public void Clear()
        {
            _header.Clear();
            _bottom.Clear();
            _current.Clear();
        }

        /// <summary>
        /// 将画刷<paramref name="target" />的内容写入到源内容中
        /// </summary>
        /// <param name="target"></param>
        internal void Combine(PageBrush target)
        {
            _header.Append(target._header);
            _bottom.Append(target._bottom);
            _current.Append(target._current);
        }

        /// <summary>
        /// 将xaml代码解析并写入画刷
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public void DrawXaml(string xaml)
        {
            var e = XamlReader.ReadComponent(xaml) as UIElement;
            if (e == null) return;
            e.Render(this);
        }

    }

}
