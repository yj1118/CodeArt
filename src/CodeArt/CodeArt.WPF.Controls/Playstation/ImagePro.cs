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

namespace CodeArt.WPF.Controls.Playstation
{
    public class ImagePro : Control
    {
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImagePro), new PropertyMetadata(Stretch.Uniform));

        /// <summary>
        /// 是否开启loading效果
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty EnabledLoadingProperty = DependencyProperty.Register("EnabledLoading", typeof(bool), typeof(ImagePro), new PropertyMetadata(false));

        /// <summary>
        /// 是否开启loading效果
        /// </summary>
        public bool EnabledLoading
        {
            get { return (bool)GetValue(EnabledLoadingProperty); }
            set { SetValue(EnabledLoadingProperty, value); }
        }


        public static readonly DependencyProperty CacheProperty = DependencyProperty.Register("Cache", typeof(bool), typeof(ImagePro), new PropertyMetadata(true));

        public bool Cache
        {
            get { return (bool)GetValue(CacheProperty); }
            set { SetValue(CacheProperty, value); }
        }

        public static readonly DependencyProperty LoadActionProperty = DependencyProperty.Register("LoadAction", typeof(Func<string, ImageSource>), typeof(ImagePro));

        public Func<string, ImageSource> LoadAction
        {
            get { return (Func<string, ImageSource>)GetValue(LoadActionProperty); }
            set { SetValue(LoadActionProperty, value); }
        }

        public static readonly DependencyProperty AutoChangeStretchProperty = DependencyProperty.Register("AutoChangeStretch", typeof(bool), typeof(ImagePro), new PropertyMetadata(false));

        /// <summary>
        /// 是否开启自动改变Stretch功能
        /// </summary>
        public bool AutoChangeStretch
        {
            get { return (bool)GetValue(AutoChangeStretchProperty); }
            set { SetValue(AutoChangeStretchProperty, value); }
        }

        public ImagePro()
        {
            this.DefaultStyleKey = typeof(ImagePro);
        }

        private Image image;
        private Loading loading;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            image = GetTemplateChild("image") as Image;
            loading = GetTemplateChild("loading") as Loading;

            if (this.EnabledLoading)
            {
                ShowLoading();
            }
            else
            {
                ShowPicture();
            }

            SourceChanged(this, new DependencyPropertyChangedEventArgs(SourceProperty, null, this.Source));
        }

        private void ShowLoading()
        {
            image.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Visible;
        }

        private void ShowPicture()
        {
            image.Visibility = Visibility.Visible;
            loading.Visibility = Visibility.Collapsed;
        }

        private void ShowError()
        {
            image.Visibility = Visibility.Collapsed;
            loading.Visibility = Visibility.Collapsed;
        }

        public static readonly DependencyProperty SourceProperty;

        /// <summary>
        /// 
        /// </summary>
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static async void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pro = d as ImagePro;
            var image = pro.image;
            if (image == null) return;
            if (e.NewValue == e.OldValue) return;

            var source = (string)e.NewValue;
            if (string.IsNullOrEmpty(source)) return;
            try
            {
                if (pro.LoadAction != null)
                {
                    var load = pro.LoadAction;
                    //使用自定义加载的方法
                    image.Source = await Task.Run<ImageSource>(() =>
                    {
                        return load(source);
                    });
                }
                else
                {
                    image.Source = await Util.LoadSource(source, pro.Cache);
                }
                if (pro.EnabledLoading)
                {
                    //显示图片
                    pro.ShowPicture();
                }

                if (pro.AutoChangeStretch)
                {
                    pro.ChangeStretch();
                }
            }
            catch (Exception)
            {
                //图片加载异常
                pro.ShowError();
            }
        }

        static ImagePro()
        {
            PropertyMetadata sourceMetadata = new PropertyMetadata(SourceChanged);
            SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ImagePro), sourceMetadata);
        }

        private void ChangeStretch()
        {
            var area = Work.Current.LogicArea;

            if (image.Source.Width > area.Width || image.Source.Height > area.Height)
            {
                image.Stretch = Stretch.Uniform;
            }
            else
            {
                image.Stretch = Stretch.None;
            }
        }

    }
}