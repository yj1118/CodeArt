using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Web.WebPages.Xaml.Script
{
    /// <summary>
    /// 脚本元素
    /// 所有的脚本元素支持统一的模式：
    /// 1.服务器端需要获取脚本元素的值或相关数据时，以属性公布
    /// 2.客户端执行的脚本动作，以方法公布
    /// </summary>
    public abstract class ScriptElement
    {
        public string Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public DTObject Metadata
        {
            get;
            private set;
        }

        public ScriptView View
        {
            get;
            private set;
        }

        internal void SetSource(ScriptView view, DTObject source)
        {
            this.Id = source.GetValue<string>("id", string.Empty);
            this.Name = source.GetValue<string>("name", string.Empty);
            this.Metadata = source.GetObject("metadata");
            this.View = view;
        }

        public ScriptElement()
        {
        }

        /// <summary>
        /// 不显示元素，且不为其保留布局空间
        /// </summary>
        public void Collapsed()
        {
            this.View.WriteCode(string.Format("{0}.hide();", this.Id));
        }

        ///// <summary>
        ///// 不显示元素，但为元素保留布局空间
        ///// </summary>
        //public void Hidden()
        //{

        //}

        /// <summary>
        /// 显示元素
        /// </summary>
        public void Visible()
        {
            this.View.WriteCode(string.Format("{0}.show();", this.Id));
        }

        /// <summary>
        /// 设置元素文本内容
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            this.View.WriteCode(string.Format("{0}.text(\"{1}\");", this.Id, text));
        }

        /// <summary>
        /// 添加样式名
        /// </summary>
        /// <param name="className"></param>
        public void AddClass(string className)
        {
            this.View.WriteCode(string.Format("{0}.addClass(\"{1}\");", this.Id, className));
        }


        /// <summary>
        /// 移除样式名
        /// </summary>
        /// <param name="className"></param>
        public void RemoveClass(string className)
        {
            this.View.WriteCode(string.Format("{0}.removeClass(\"{1}\");", this.Id, className));
        }

    }
}
