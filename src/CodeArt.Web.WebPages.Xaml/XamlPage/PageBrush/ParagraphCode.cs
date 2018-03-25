using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Text;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 页面画刷段落代码，每个段落的代码都不能重复，该类内部使用
    /// </summary>
    internal class PageBrushParagraphCode
    {
        private List<string> _data = new List<string>();

        private bool _forceFilterRepeat = false; //强制过滤重复


        public PageBrushParagraphCode(bool forceFilterRepeat)
        {
            _forceFilterRepeat = forceFilterRepeat;
        }

        public void Append(PageBrushParagraphCode code)
        {
            var target = code._data;
            foreach(var item in target)
            {
                this.Append(item);
            }
        }

        public void Append(string code)
        {
            if (_forceFilterRepeat && Contains(code)) return;
            _data.Add(code);
        }

        public void AppendLine(string code)
        {
            if (_forceFilterRepeat && Contains(code)) return;
            _data.Add(code);
            _data.Add(Environment.NewLine);
        }

        /// <summary>
        /// 回退最后一批空格、换行符等
        /// </summary>
        /// <returns></returns>
        public void Backspace()
        {
            var index = _data.Count - 1;
            var end = false;
            while(index >= 0)
            {
                using (var temp = StringPool.Borrow())
                {
                    var code = temp.Item;
                    code.Append(_data[index]);

                    while (code.Length > 0)
                    {
                        var c = code[code.Length - 1];
                        if (c == '\r' || c == '\n' || c == ' ') code.Length--;
                        else
                        {
                            _data[index] = code.ToString();
                            end = true;
                            break;
                        }
                    }
                }

                if (end) break;
                _data.RemoveAt(index); //代码如果能成功运行到此处，代表code已经被减完了，所以移除
                index--;

            }
        }

        /// <summary>
        /// 回退字符
        /// </summary>
        /// <param name="count">需要回退的字符个数</param>
        public void Backspace(int count)
        {
            var index = _data.Count - 1;
            var end = false;
            while (index >= 0)
            {
                using (var temp = StringPool.Borrow())
                {
                    var code = temp.Item;
                    code.Append(_data[index]);

                    while (code.Length > 0)
                    {
                        var c = code[code.Length - 1];
                        if (c == '\r' || c == '\n' || c == ' ') code.Length--; //自动过滤空白字符
                        else
                        {
                            code.Length--;
                            count--;

                            if (count == 0)
                            {
                                _data[index] = code.ToString();
                                end = true;
                                break;
                            }
                        }
                    }
                }

                if (end) break;
                _data.RemoveAt(index); //代码如果能成功运行到此处，代表code已经被减完了，所以移除
                index--;
            }
        }

        public override string ToString()
        {
            return string.Join(string.Empty, _data);
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(string code)
        {
            if (string.IsNullOrEmpty(code.Trim())) return false; //空白字符串不算重复
            return _data.Contains(code);
        }

    }
}
