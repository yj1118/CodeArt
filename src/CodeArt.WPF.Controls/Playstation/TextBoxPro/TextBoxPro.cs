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

using CodeArt.WPF.UI;
using CodeArt.DTO;

namespace CodeArt.WPF.Controls.Playstation
{
    public class TextBoxPro : Input
    {
        /// <summary>
        /// 输入框中的提示
        /// </summary>
        public static readonly DependencyProperty TipProperty = DependencyProperty.Register("Tip", typeof(string), typeof(TextBoxPro));

        public string Tip
        {
            get
            {
                return (string)GetValue(TipProperty);
            }
            set
            {
                SetValue(TipProperty, value);
            }
        }

        public static DependencyProperty MinLinesProperty = DependencyProperty.Register("MinLines", typeof(int), typeof(TextBoxPro), new PropertyMetadata(1));

        public int MinLines
        {
            get
            {
                return (int)GetValue(MinLinesProperty);
            }
            set
            {
                SetValue(MinLinesProperty, value);
            }
        }

        public static DependencyProperty MaxLinesProperty = DependencyProperty.Register("MaxLines", typeof(int), typeof(TextBoxPro), new PropertyMetadata(1));

        public int MaxLines
        {
            get
            {
                return (int)GetValue(MaxLinesProperty);
            }
            set
            {
                SetValue(MaxLinesProperty, value);
            }
        }

        public static DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBoxPro), new PropertyMetadata(0));

        public int MaxLength
        {
            get
            {
                return (int)GetValue(MaxLengthProperty);
            }
            set
            {
                SetValue(MaxLengthProperty, value);
            }
        }

        public static DependencyProperty MinLengthProperty = DependencyProperty.Register("MinLength", typeof(int), typeof(TextBoxPro), new PropertyMetadata(0));

        public int MinLength
        {
            get
            {
                return (int)GetValue(MinLengthProperty);
            }
            set
            {
                SetValue(MinLengthProperty, value);
            }
        }

        public static DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof(double), typeof(TextBoxPro), new PropertyMetadata((double)28));

        public double LineHeight
        {
            get
            {
                return (double)GetValue(LineHeightProperty);
            }
            set
            {
                SetValue(LineHeightProperty, value);
            }
        }

        public static DependencyProperty LineCountProperty = DependencyProperty.Register("LineCount", typeof(int), typeof(TextBoxPro), new PropertyMetadata(1));

        public int LineCount
        {
            get
            {
                return (int)GetValue(LineCountProperty);
            }
            set
            {
                SetValue(LineCountProperty, value);
            }
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBoxPro), new PropertyMetadata(string.Empty));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public TextBoxPro()
        {
            this.DefaultStyleKey = typeof(TextBoxPro);
        }

        private TextBox _input;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _input = GetTemplateChild("input") as TextBox;
            this.MakeChildsLoaded(_input);
            _input.TextChanged += OnInputTextChanged;
        }

        private void OnInputTextChanged(object sender, TextChangedEventArgs e)
        {
            this.LineCount = _input.LineCount;

            //当文本更改时，需要传递更改了，这样可以用于双向绑定
            BindingExpression be = _input.GetBindingExpression(TextBox.TextProperty);
            if(be!= null)
            {
                be.UpdateSource();
            }
        }

        public override void Write(DTObject data)
        {
            var value = _input.Text;
            data.SetValue(this.MemberName, value);
        }

        public string Get()
        {
            return _input.Text;
        }

        public void Set(string value)
        {
            _input.Text = value;
        }


        public override void Clear()
        {
            Set(string.Empty);
        }

        public override void Validate(ValidationResult result)
        {
            var value = this.Get();
            if (this.Required && value.Length == 0) result.AddError(string.Format("{0}{1}", Strings.PleaseEnter, this.Label));
            if (value.Length < this.MinLength) result.AddError(string.Format(Strings.CanNotBeLessCharacters, this.Label, this.MinLength));
            if (this.MaxLength > 0 && value.Length > this.MaxLength) result.AddError(string.Format(Strings.CanNotBeLongerCharacters, this.Label, this.MaxLength));
        }
    }
}