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
using Microsoft.Win32;
using System.ComponentModel;

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


        #region  浏览器设置

        /// <summary>  
        /// 修改注册表信息来兼容当前程序  
        /// </summary>  
        public static void SetWebBrowser(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio  
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称  
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值  
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行  
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",
                appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser  
            //不晓得设置有什么用  
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",
                appName, 1, RegistryValueKind.DWord);


            //Registry.SetValue(featureControlRegKey + "FEATURE_AJAX_CONNECTIONEVENTS",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_GPU_RENDERING",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_WEBOC_DOCUMENT_ZOOM",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_NINPUT_LEGACYMODE",  
            //    appName, 0, RegistryValueKind.DWord);
        }


        /// <summary>  
        /// 获取浏览器的版本  
        /// </summary>  
        /// <returns></returns>  
        public static int GetBrowserVersion()
        {
            int browserVersion = 0;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }
            //如果小于7  
            if (browserVersion < 7)
            {
                throw new ApplicationException("不支持的浏览器版本!");
            }
            return browserVersion;
        }


        /// <summary>  
        /// 通过版本得到浏览器模式的值  
        /// </summary>  
        /// <param name="browserVersion"></param>  
        /// <returns></returns>  
        static private UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;

        }


        #endregion



    }
}
