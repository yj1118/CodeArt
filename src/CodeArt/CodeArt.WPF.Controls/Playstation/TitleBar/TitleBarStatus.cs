using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// 标题栏的状态
    /// </summary>
    public class TitleBarStatus
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

        public double IconWidth
        {
            get;
            private set;
        }

        public double IconHeight
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public TitleBarStatus(string name, string icon, string description)
            : this(name, icon, 30, 30, description)
        {
        }

        public TitleBarStatus(string name, string icon, double iconWidth, double iconHeight, string description)
        {
            this.Name = name;
            this.Icon = icon;
            this.IconWidth = iconWidth;
            this.IconHeight = iconHeight;
            this.Description = description;
        }
    }
}
