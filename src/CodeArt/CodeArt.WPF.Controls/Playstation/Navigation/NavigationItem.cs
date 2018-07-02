using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeArt.WPF.Controls.Playstation
{
    public class NavigationItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Image { get; set; }

        public string Text { get; set; }

        public string SubText { get; set; }

        /// <summary>
        /// 创建页面的方法
        /// </summary>
        public Func<UIElement> Create { get; set; }

    }
}
