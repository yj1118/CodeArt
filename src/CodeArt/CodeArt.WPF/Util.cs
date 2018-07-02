using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

using CodeArt.Util;
using CodeArt.Net;
using System.Windows.Threading;

namespace CodeArt.WPF
{
    public static class Util
    {
        public static async Task<ImageSource> LoadSource(string source, bool cache)
        {
            if (cache)
            {
                if (_imageCaches.TryGetValue(source, out var image))
                {
                    return image;
                }
            }

            return await Task.Run<ImageSource>(() =>
            {
                ImageSource image = null;
                //打开文件流
                using (var stream = OpenStream())
                {
                    image = stream.GetImageSource();
                }

                if (cache)
                {
                    _imageCaches.TryAdd(source, image);
                }

                Stream OpenStream()
                {
                    if (source.StartsWith("http:") || source.StartsWith("https:"))
                    {
                        var data = HttpCommunicator.Instance.Get(source);
                        return new MemoryStream(data);
                    }
                    else
                    {
                        var uri = new Uri(source, UriKind.RelativeOrAbsolute);
                        string path = uri.IsAbsoluteUri ? uri.AbsolutePath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, source.Replace("/", "\\").Trim('\\'));

                        if (!File.Exists(path)) return null; 
                        return File.OpenRead(path);
                    }
                }

                return image;

            });
        }

        private static ConcurrentDictionary<string, ImageSource> _imageCaches = new ConcurrentDictionary<string, ImageSource>();


        public static System.Windows.Media.Brush GetBrush(string color)
        {
            var converter = new BrushConverter();
            return (System.Windows.Media.Brush)converter.ConvertFromString(color);
        }

        /// <summary>
        /// 重启系统
        /// </summary>
        public static void Restart()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public static void Shutdown()
        {
            Application.Current.Shutdown();
        }


        private static DispatcherOperationCallback
             exitFrameCallback = new DispatcherOperationCallback(ExitFrame);

        /// <summary>
        /// 刷新WPF界面
        /// </summary>
        public static void Refresh()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation =
                Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }
        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }

    }
}
