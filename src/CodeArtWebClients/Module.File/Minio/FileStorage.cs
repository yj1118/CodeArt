using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

using CodeArt.IO;
using CodeArt.Util;
using CodeArt.Drawing;
using CodeArt.Drawing.Imaging;

namespace Module.File.Minio
{
    /// <summary>
    /// 该对象是线程安全的，可以多线程同时使用
    /// </summary>
    public class FileStorage
    {
        private FileStorage()
        {
        }
        
        public string GetPath(string key)
        {
            return GetPath(key, null);
        }

        public static string GetPath(string key,string root)
        {
            const string configName = "fileStorage";
            string folder = ConfigurationManager.AppSettings[configName];
            if (folder == null) throw new ApplicationException(string.Format(Strings.CannotWriteTemporaryFile, configName));
            
            if(!string.IsNullOrEmpty(root))
            {
                folder = Path.Combine(folder, root);
            }

            MD5 md = new MD5CryptoServiceProvider();
            string code = BitConverter.ToString(md.ComputeHash(new UnicodeEncoding().GetBytes(key)));
            string balancedFolder = code.Substring(0, 8).Replace("-", "\\");
            balancedFolder = Path.Combine(folder, balancedFolder);
            balancedFolder = Path.Combine(balancedFolder, code);

            if (!Directory.Exists(balancedFolder)) Directory.CreateDirectory(balancedFolder);
            return Path.Combine(balancedFolder, key);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public string Save(Stream content, string clientFileName)
        {
            string extension = IOUtil.GetExtension(clientFileName);
            string id = string.Format("{0}.{1}", Guid.NewGuid().ToString("n"), extension);
            var path = GetPath(id);

            var bytes = new byte[content.Length];
            content.Read(bytes, 0, bytes.Length);

            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            return id;
        }


        /// <summary>
        /// 将文件移动至存储区
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string MoveAndCompress(string fileName)
        {
            //由于根据文件流判断后缀，不同的文件很容易出问题，所以暂时用简单的方式判断
            string extension = IOUtil.GetExtension(fileName);
            //using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //{
            //    extension = IOUtil.GetExtension(fs, clientFileName);
            //}

            string id = string.Format("{0}.{1}", Guid.NewGuid().ToString("n"), extension);
            var path = GetPath(id);

            //对图片进行最终处理
            End(fileName, path, extension);

            return id;
        }

        /// <summary>
        /// ,将路径<paramref name="fileName"/>上的文件,压缩后替换至<paramref name="fileKey"/>对应的文件,fileKey，请确保唯一性
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileKey"></param>
        /// <returns>true:替换了文件，false:新增的文件</returns>
        public void MoveAndCompress(string fileName, string fileKey)
        {
            var extension = IOUtil.GetExtension(fileKey);
            if (string.IsNullOrEmpty(extension)) throw new InvalidOperationException("fileKey需要指定文件后缀名");

            var path = GetPath(fileKey);

            //判断虚拟文件是否存在
            if(System.IO.File.Exists(path))
            {
                IOUtil.Delete(path);

            }

            //对图片进行最终处理
            End(fileName, path, extension);
        }

        /// <summary>
        /// 支持 thumb = fileKey x width x height x cutType ,就可以直接生成缩略图
        /// </summary>
        /// <param name="tempFileName"></param>
        /// <param name="targetFileName"></param>
        /// <param name="extension"></param>
        private void End(string tempFileName,string targetFileName,string extension)
        {
            var thumb = default((string,int,int,int,int));
            if (TryGetThumbInfo(ref thumb))
            {
                //存缩略图
                SaveThumb(tempFileName, thumb);
                return;
            }

            System.IO.File.Move(tempFileName, targetFileName);
        }

        private bool TryGetThumbInfo(ref (string fileKey, int Width, int Height, int CutType,int highQuality) thumb)
        {
            var thumbString = HttpContext.Current.Request.Headers["thumb"];
            if (string.IsNullOrEmpty(thumbString)) return false;
            var temp = thumbString.Split('x');
            thumb = (temp[0], int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]),int.Parse(temp[4]));
            return true;
        }

        private void SaveThumb(string tempFileName, (string fileKey, int Width, int Height, int CutType, int HighQuality) thumb)
        {
            string targetFileName = GetThumbPath(thumb.fileKey, thumb.Width, thumb.Height, thumb.CutType, thumb.HighQuality);
            IOUtil.Delete(targetFileName);
            System.IO.File.Move(tempFileName, targetFileName);
        }

        public void Delete(string key)
        {
            var fileName = GetPath(key);
            System.IO.File.Delete(fileName);
        }

        #region 读取

        private string ParseId(string key)
        {
            return key.Substring(8);
        }

        private string GetThumbKey(string key, int width, int height, int cutType, int quality)
        {
            return (width <= 0 && height <= 0) ? key : string.Format("{0}_{1}_{2}_{3}_{4}", key, width, height, cutType, quality);
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="key"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="highQuality">0-100</param>
        /// <returns></returns>
        public Stream LoadByImage(string key, int width, int height, int cutType,int quality)
        {
            var sourceFileName = GetPath(key);

            if (width <= 0 && height <= 0 && quality >=100)//输出原始图片
            {
                return new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            else
            {
                var fileName = GetThumbPath(key, width, height, cutType, quality);
                IOUtil.CreateFileDirectory(fileName);

                if (System.IO.File.Exists(fileName))
                    return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                else
                {
                    lock (_lockObject)
                    {
                        if (System.IO.File.Exists(fileName))
                            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        else
                        {
                            return ThumbAndSaveImage(sourceFileName, fileName, width, height, cutType, quality);
                        }
                    }
                }
            }
        }

        private object _lockObject = new object();

        private Stream ThumbAndSaveImage(string sourceFileName, string fileName, int width, int height, int cutType, int quality)
        {
            string method = "cover";
            switch (cutType)
            {
                case 1: method = "fit"; break;
                case 2: method = "cover"; break;
                case 3: method = "stretch"; break;
            }

            ImageUtil.Resize(sourceFileName, fileName, width, height, quality, method);

            //输出文件
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }


        public Stream Load(string key)
        {
            var sourceFileName = GetPath(key);

            return new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private string GetThumbPath(string key,int width,int height,int cutType,int highQuality)
        {
            var thumbKey = GetThumbKey(key, width, height, cutType, highQuality);

            return GetPath(thumbKey, "thumb");
        }

        #endregion

        public readonly static FileStorage Instance = new FileStorage();

    }
}
