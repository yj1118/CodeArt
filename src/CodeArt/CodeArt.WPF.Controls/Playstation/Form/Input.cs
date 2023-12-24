using System;
using System.Collections;
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

using CodeArt.DTO;


namespace CodeArt.WPF.Controls.Playstation
{
    public abstract class Input : Control, IInput
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(Input));

        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public static readonly DependencyProperty MainWidthProperty = DependencyProperty.Register("MainWidth", typeof(double), typeof(Input));

        public double MainWidth
        {
            get
            {
                return (double)GetValue(MainWidthProperty);
            }
            set
            {
                SetValue(MainWidthProperty, value);
            }
        }

        public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register("LabelWidth", typeof(double), typeof(Input));

        public double LabelWidth
        {
            get
            {
                return (double)GetValue(LabelWidthProperty);
            }
            set
            {
                SetValue(LabelWidthProperty, value);
            }
        }

        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(Input), new PropertyMetadata((double)28));

        public double LabelFontSize
        {
            get
            {
                return (double)GetValue(LabelFontSizeProperty);
            }
            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        public static readonly DependencyProperty LabelLineHeightProperty = DependencyProperty.Register("LabelLineHeight", typeof(double), typeof(Input), new PropertyMetadata((double)28));

        public double LabelLineHeight
        {
            get
            {
                return (double)GetValue(LabelLineHeightProperty);
            }
            set
            {
                SetValue(LabelLineHeightProperty, value);
            }
        }


        public static readonly DependencyProperty MainHeightProperty = DependencyProperty.Register("MainHeight", typeof(double), typeof(Input));

        public double MainHeight
        {
            get
            {
                return (double)GetValue(MainHeightProperty);
            }
            set
            {
                SetValue(MainHeightProperty, value);
            }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(Input));

        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }


        #region 验证

        /// <summary>
        /// 是否必填
        /// </summary>
        public static readonly DependencyProperty RequiredProperty = DependencyProperty.Register("Required", typeof(bool), typeof(Input));

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required
        {
            get
            {
                return (bool)GetValue(RequiredProperty);
            }
            set
            {
                SetValue(RequiredProperty, value);
            }
        }

        public abstract void Validate(ValidationResult result);

        #endregion


        public string MemberName
        {
            get;
            set;
        }

        public event InputValueChangedEventHandler ValueChanged;

        protected void RaiseValueChanged(object sender, InputValueChangedEventArgs e)
        {
            if (this.ValueChanged != null)
                this.ValueChanged(sender, e);
        }

        /// <summary>
        /// 清理值
        /// </summary>
        public abstract void Clear();

        public abstract void Write(DTObject data);

        public abstract void Read(DTObject data);

    }
}
