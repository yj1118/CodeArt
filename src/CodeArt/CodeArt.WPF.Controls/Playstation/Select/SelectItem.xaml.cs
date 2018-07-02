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

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// SelectItem.xaml 的交互逻辑
    /// </summary>
    public partial class SelectItem : ContentControl
    {
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(SelectItems), typeof(SelectItem), new PropertyMetadata(null, OwnerChanged));
        /// <summary>
        /// 
        /// </summary>
        public SelectItems Owner
        {
            get
            {
                return (SelectItems)GetValue(OwnerProperty);
            }
            set
            {
                SetValue(OwnerProperty, value);
            }
        }

        private static void OwnerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var si = d as SelectItem;
            var owner = e.NewValue as SelectItems;
            si.Selected = owner.ContainsValue(si.Data);
            si._sign.IsRadio = owner.IsRadio;
        }


        public bool Selected
        {
            get
            {
                return this.Data.IsSelected;
            }
            set
            {
                this.Data.IsSelected = value;
            }
        }


        public SelectItem()
        {
            InitializeComponent();
        }

        private Range _range;
        private SelectSign _sign;
        private ImagePro _disabled;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Init();
        }

        private void Init()
        {
            _range = GetTemplateChild("range") as Range;
            _sign = GetTemplateChild("sign") as SelectSign;
            _disabled = GetTemplateChild("disabled") as ImagePro;

            this.MakeChildsLoaded(_sign);

            this.MouseUp += OnSelectItem;
        }

        public TextItem Data
        {
            get
            {
                return this.DataContext as TextItem;
            }
        }

        private void OnSelectItem(object sender, MouseButtonEventArgs e)
        {
            bool selected = !this.Selected;

            var data = this.Data;
            if (data == null) return;

            var owner = this.Owner;
            if(owner.IsRadio)
            {
                if(selected)
                {
                    owner.ClearValues();
                    owner.AddValue(data);
                }
                else
                {
                    owner.RemoveValue(data);
                }
            }
            else
            {
                if (selected)
                {
                    owner.AddValue(data);
                }
                else
                {
                    owner.RemoveValue(data);
                }
            }
        }

        public static readonly DependencyProperty DisabledProperty = DependencyProperty.Register("Disabled", typeof(bool), typeof(SelectItem), new PropertyMetadata(false, DisabledChanged));


        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        private static void DisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (SelectItem)d;
            var value = (bool)e.NewValue;
            if (item._range == null) item.ApplyTemplate();
            item._range.Status = value ? RangeStatus.Disabled : RangeStatus.Enabled;
            if(value)
            {
                item._sign.Visibility = Visibility.Collapsed;
                item._disabled.Visibility = Visibility.Visible;
            }
            else
            {
                item._sign.Visibility = Visibility.Visible;
                item._disabled.Visibility = Visibility.Collapsed;
            }
        }


    }
}
