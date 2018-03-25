using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Markup;

using CodeArt.WPF.UI;
using CodeArt.DTO;


namespace CodeArt.WPF.Controls.Playstation
{
    [ContentProperty("Content")]
    public class Form : FormBase
    {
        /// <summary>
        /// 操作处理完毕的提示信息
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(Form), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }
            set { SetValue(OperatedTipProperty, value); }
        }


        public Form()
        {
            this.DefaultStyleKey = typeof(Form);
        }
    }
}