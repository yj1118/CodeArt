using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Media;

using CodeArt.WPF;
using CodeArt.Util;
using System.Windows.Interop;

namespace CodeArt.WPF
{
    public static class Extensions
    {
        #region 应用皮肤

        /// <summary>
        /// 应用皮肤
        /// </summary>
        /// <param name="app"></param>
        /// <param name="skinUri"></param>
        public static void ApplySkin(this Application app, Uri skinUri)
        {
            //加载皮肤资源
            ResourceDictionary skin = Application.LoadComponent(skinUri) as ResourceDictionary;
            MergeStyles(app.Resources, skin);

            //var skinBaseUri = skin.GetBaseUri();
            //var merged = app.Resources.MergedDictionaries;
            //ResourceDictionary old = null;
            //foreach (var item in merged)
            //{
            //    if (item.GetBaseUri() == skinBaseUri)
            //    {
            //        old = item;
            //        break;
            //    }
            //}
            //if (old != null)
            //{
            //    merged.Remove(old);
            //}

            //merged.Add(skin);


        }

        /// <summary>
        /// 该算法精确到setter,可以保留原始所有样式的同时，替换或者补充后来加的样式里面的setter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="append"></param>
        private static void MergeStyles(ResourceDictionary source, ResourceDictionary append)
        {
            foreach (string key in append.Keys)
            {
                //检索追加的资源中的每一个style
                var targetStyle = append[key] as Style;
                var result = FindStyle(source, key);
                var sourceStyle = result.style;
                var sourceResource = result.resource;
                if (result.style == null)
                {
                    //原始资源中没有，那么直接追加
                    source.Add(key, targetStyle);
                }
                else
                {
                    //原始资源中有，那么合并setter
                    Style style = new Style(sourceStyle.TargetType, sourceStyle.BasedOn);
                    foreach (var sourceSetter in sourceStyle.Setters)
                    {
                        style.Setters.Add(sourceSetter);
                    }

                    foreach (Setter targetSetter in targetStyle.Setters)
                    {
                        var same = style.Setters.FirstOrDefault((t) =>
                        {
                            return (t as Setter).Property.Name.EqualsIgnoreCase(targetSetter.Property.Name);
                        }) as Setter;
                        if (same != null)
                        {
                            style.Setters.Remove(same);
                        }
                        style.Setters.Add(targetSetter);
                    }

                    //移除老的style,将新生成的style追加到资源中
                    sourceResource.Remove(sourceStyle);
                    sourceResource.Add(key, style);
                }
            }
        }

        private static (Style style, ResourceDictionary resource) FindStyle(ResourceDictionary resource, string styleKey)
        {
            if (resource.Contains(styleKey))
            {
                return (resource[styleKey] as Style, resource);
            }

            foreach (var item in resource.MergedDictionaries)
            {
                var result = FindStyle(item, styleKey);
                if (result.style != null) return result;
            }
            return (null, null);
        }

        #endregion


        public static Uri GetBaseUri(this ResourceDictionary rd)
        {
            return ((IUriContext)rd).BaseUri;
        }


        public static BitmapImage ToImage(this Bitmap bmp)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream ms = bmp.ToStream())
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }

        public static MemoryStream ToStream(this Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms;
        }

        public static T GetParent<T>(this DependencyObject obj) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                var p = parent as T;
                if (p != null) return p;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// 查找子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static T GetChild<T>(this DependencyObject obj, string childName) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && (string.IsNullOrEmpty(childName) || child.GetValue(FrameworkElement.NameProperty).ToString() == childName))
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = GetChild<T>(child, childName);
                    if (childOfChild != null) return childOfChild;
                }
            }
            return null;
        }

        public static List<T> GetChilds<T>(this DependencyObject obj) where T : class
        {
            List<T> result = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var item = VisualTreeHelper.GetChild(obj, i);
                var child = item as T;
                if (child != null) result.Add(child);
                result.AddRange(GetChilds<T>(item));//指定集合的元素添加到List队尾  
            }
            return result;
        }

        public static void EachChilds(this DependencyObject obj, Func<DependencyObject, bool> action)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var item = VisualTreeHelper.GetChild(obj, i);
                var isContinue = action(item);
                if (isContinue)
                {
                    //如果继续分析，那么遍历item的子节点
                    EachChilds(item, action);
                }
            }
        }


        /// <summary>
        /// 确保子项被加载
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childs"></param>
        public static void MakeChildsLoaded(this DependencyObject parent, params FrameworkElement[] childs)
        {
            foreach (var child in childs)
            {
                child.ApplyTemplate();
            }
        }


        public static ImageSource GetImageSource(this Stream stream)
        {
            if (stream == null || stream.Length == 0) return null;
            BitmapImage image = new BitmapImage();
            //打开文件流
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();//这一句很重要，少了UI线程就不认了

            return image;
        }

        /// <summary>
        /// 获取窗口所属的屏幕信息
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static System.Windows.Forms.Screen GetScreen(this Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            return System.Windows.Forms.Screen.FromHandle(handle);
        }

        /// <summary>
        /// 判断子控件是否在父控件中可见
        /// </summary>
        /// <param name="child">子控件</param>
        /// <param name="parent">父控件</param>
        /// <returns></returns>
        public static bool IsChildVisibleInParent(this FrameworkElement child, FrameworkElement parent)
        {
            var childTransform = child.TransformToAncestor(parent);
            var childRectangle = childTransform.TransformBounds(new Rect(new System.Windows.Point(0, 0), child.RenderSize));
            var ownerRectangle = new Rect(new System.Windows.Point(0, 0), parent.RenderSize);
            return ownerRectangle.IntersectsWith(childRectangle);
        }


    }
}
