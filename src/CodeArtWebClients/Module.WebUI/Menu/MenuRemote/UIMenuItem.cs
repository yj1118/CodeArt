using System.Collections.Generic;
using System.Text;

namespace Module.WebUI
{
    public sealed class UIMenuItem
    {
        public string Name
        {
            get;
            private set;
        }

        public string Icon
        {
            get;
            private set;
        }

        public string IconFontSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否加入快捷方式
        /// </summary>
        public string Tags
        {
            get;
            private set;
        }

        public string Code
        {
            get;
            private set;
        }

        private List<UIMenuItem> _childs = new List<UIMenuItem>();
        public IList<UIMenuItem> Childs
        {
            get
            {
                return _childs;
            }
        }

        public void AddChilds(IList<UIMenuItem> items)
        {
            _childs.AddRange(items);
        }

        public void AddChild(UIMenuItem item)
        {
            _childs.Add(item);
        }

        public UIMenuItem(string name,string icon,string iconFontSize, string tags, string code)
        {
            this.Name = name;
            this.Icon = icon;
            this.IconFontSize = iconFontSize;
            this.Tags = tags;
            this.Code = code;
        }

        public UIMenuItem(string name)
            : this(name, string.Empty,string.Empty, string.Empty, string.Empty)
        {
        }

        public string ToJSON()
        {
            StringBuilder code = new StringBuilder("{");
            code.AppendFormat("name:'{0}'", this.Name);
            if(!string.IsNullOrEmpty(this.Icon)) code.AppendFormat(",icon:'{0}'", this.Icon);
            if (!string.IsNullOrEmpty(this.IconFontSize)) code.AppendFormat(",iconFontSize:'{0}'", this.IconFontSize);
            
            if (!string.IsNullOrEmpty(this.Tags))
            {
                code.Append(",tags:[");
                var temp = this.Tags.Split(',');
                foreach(var tag in temp)
                {
                    code.AppendFormat("'{0}',", tag);
                }
                if(temp.Length > 0) code.Length--;
                code.Append("]");
            }
            if (!string.IsNullOrEmpty(this.Code)) code.AppendFormat(",'code':{0}", this.Code);
            if (this.Childs.Count > 0)
            {
                code.Append(",childs:[");
                foreach (var child in this.Childs)
                {
                    code.AppendFormat("{0},",child.ToJSON());
                }
                code.Length--;
                code.Append("]");
            }
            code.Append("}");
            return code.ToString();
        }

        public static UIMenuItem Empty = new UIMenuItem(string.Empty,string.Empty,string.Empty, string.Empty, string.Empty);

    }
}
