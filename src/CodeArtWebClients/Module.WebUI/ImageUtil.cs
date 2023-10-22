using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Text.RegularExpressions;

using CodeArt.ServiceModel;
using CodeArt.Web;

namespace Module.WebUI
{
    public static class ImageUtil
    {
        private static string[] _images = { "jpg", "jpeg", "png", "gif", "bmp", "tiff", "svg" };

        public static string GetCoverUrl(Guid coverId, int width, int height)
        {
            return DomainUtil.GetUrl("file", string.Format("/cover-{0}-{1}-{2}", coverId.ToString("N"), width, height));
        }

        [Obsolete("被GetDynamicUrl(category)取代")]
        public static string GetDynamicUrl(string storeKey, int width = 122, int height = 91, int cutType = 1)
        {
            if (!string.IsNullOrEmpty(storeKey)) return GetUrl("thumbnailUrl", string.Format("/image.htm?w={0}&h={1}&c={2}&key={3}", width, height, cutType, storeKey));
            else
            {
                return GetUrl("thumbnailUrl", string.Format("/images/default/{0}-{1}.jpg", width, height));
            }
        }

        public static string GetDynamicUrl(string category, string storeKey, int width = 122, int height = 91, int cutType = 1,int quality = 30)
        {
            if (!string.IsNullOrEmpty(storeKey)) return GetUrl("thumbnailUrl", string.Format("/image.htm?w={0}&h={1}&c={2}&key={3}&q={4}", width, height, cutType, storeKey, quality));
            else
            {
                return GetUrl("thumbnailUrl", string.Format("/images/default/{0}-{1}-{2}.jpg", category, width, height));
            }
        }

        public static string GetFileUrl(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return GetUrl("file", string.Format("/file-{0}", key));

        }

        //public static string GetItemThumbUrl(Guid storeKey)
        //{
        //    if (string.IsNullOrEmpty(fileKey)) return string.Empty;

        //    if (IsImage(fileKey))
        //    {
        //        return GetDynamicUrl(fileKey, 122, 91, 2);
        //    }
        //    // 其他类型的文件                  
        //    else
        //    {
        //        return GetDynamicUrl(fileKey, 122, 91, 2);
        //    }
        //}

        //public static string GetItemUrl(string fileKey)
        //{
        //    if (string.IsNullOrEmpty(fileKey)) return string.Empty;

        //    if (IsImage(fileKey))
        //    {
        //        return GetDynamicUrl(fileKey, 0, 0);
        //    }
        //    // 其他类型的文件                  
        //    else
        //    {
        //        return GetFileUrl(fileKey);
        //    }
        //}

        //public static bool IsImage(string fileKey)
        //{
        //    var ext = Path.GetExtension(fileKey).Trim('.');
        //    return _images.Contains(ext);
        //}

        ///// <summary>
        ///// 获取默认图片
        ///// </summary>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        ///// <param name="cutType"></param>
        ///// <returns></returns>
        //public static string GetDefaultUrl(int width = 122, int height = 91)
        //{
        //    return GetDynamicUrl(string.Empty, width, height);
        //}


        //public static string GetUserPhoto(Guid fileId, int width, int height, int cutType = 2)
        //{
        //    if (fileId == Guid.Empty) return string.Empty;

        //    var result = ServiceContext.Invoke("getFile", (arg) => {
        //        arg["fileId"] = fileId;
        //    });

        //    var key = result.GetValue<string>("fileKey", string.Empty);

        //    if (string.IsNullOrEmpty(key)) return string.Empty;
        //    else
        //    {
        //        return GetUrl("file", string.Format("/image-{0}-{1}-{2}-{3}", width, height, cutType, key));
        //    }
        //}


        public static string GetUrl(string domainKey, string path)
        {
            var domain = GetDomain(domainKey);
            return WebUtil.Combine(domain, path);
        }

        public static string GetDomain(string domainKey, bool throwError = true)
        {
            //var domain = Application.Current.FindResource(domainKey, false) as string;
            var domain = string.Empty;
            if (string.IsNullOrEmpty(domain)) domain = System.Configuration.ConfigurationManager.AppSettings[domainKey];
            if (string.IsNullOrEmpty(domain) && throwError) throw new WebException("没有找到" + domainKey + "的资源配置");
            return domain;
        }

        public static string ParseKey(string url)
        {
#if (DEBUG)
            var equalPointer = url.IndexOf("=");
            if (equalPointer != -1)
            {
                var dotPointer = url.IndexOf("&");
                return url.Substring(equalPointer + 1, dotPointer - equalPointer - 1);
            }
            else
            {
                var pointer = url.LastIndexOf("-");
                return url.Substring(pointer + 1);
            }
#else
                var pointer = url.LastIndexOf("-");
                return url.Substring(pointer + 1);
#endif
        }
    }
}
