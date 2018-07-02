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


namespace Module.File
{
    /// <summary>
    /// 该对象是线程安全的，可以多线程同时使用
    /// </summary>
    public class FileStorage
    {
        private FileStorage()
        {
        }

        private string GetPath(string id)
        {
            const string configName = "fileStorage";
            string folder = ConfigurationManager.AppSettings[configName];
            if (folder == null) throw new ApplicationException(string.Format(Strings.CannotWriteTemporaryFile, configName));

            MD5 md = new MD5CryptoServiceProvider();
            string code = BitConverter.ToString(md.ComputeHash(new UnicodeEncoding().GetBytes(id)));
            string balancedFolder = code.Substring(0, 8).Replace("-", "\\");
            balancedFolder = Path.Combine(folder, balancedFolder);
            balancedFolder = Path.Combine(balancedFolder, code);

            if (!Directory.Exists(balancedFolder)) Directory.CreateDirectory(balancedFolder);
            return Path.Combine(balancedFolder, id);
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
        public string Move(string fileName)
        {
            //由于根据文件流判断后缀，不同的文件很容易出问题，所以暂时用简单的方式判断
            string extension = IOUtil.GetExtension(fileName);
            //using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //{
            //    extension = IOUtil.GetExtension(fs, clientFileName);
            //}

            string id = string.Format("{0}.{1}", Guid.NewGuid().ToString("n"), extension);
            var path = GetPath(id);

            System.IO.File.Move(fileName, path);

            return id;
        }

        /// <summary>
        /// ,将路径<paramref name="fileName"/>上的文件,替换至<paramref name="fileKey"/>对应的文件,fileKey，请确保唯一性
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileKey"></param>
        /// <returns>true:替换了文件，false:新增的文件</returns>
        public void Move(string fileName, string fileKey)
        {
            var extension = IOUtil.GetExtension(fileKey);
            if (string.IsNullOrEmpty(extension)) throw new InvalidOperationException("fileKey需要指定文件后缀名");

            var path = GetPath(fileKey);

            //判断虚拟文件是否存在
            if(System.IO.File.Exists(path))
            {
                IOUtil.Delete(path);

            }
            System.IO.File.Move(fileName, path);
        }


        public void Delete(string key)
        {
            var fileName = ParsePath(key);
            System.IO.File.Delete(fileName);
        }

        #region 读取

        private string ParseId(string key)
        {
            return key.Substring(8);
        }

        private string ParsePath(string key)
        {
            return GetPath(key);
        }

        private string GetThumbKey(string key, int width, int height, int cutType)
        {
            return (width <= 0 && height <= 0) ? key : string.Format("{0}_{1}_{2}_{3}", key, width, height, cutType);
        }

        private object _lockObject = new object();

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="key"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="release"></param>
        /// <returns></returns>
        public Stream LoadByImage(string key, int width, int height, int cutType)
        {
            var sourceFileName = ParsePath(key);

            if (width <= 0 && height <= 0)//输出原始图片
            {
                return new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            else
            {
                var thumbKey = GetThumbKey(key, width, height, cutType);

                var f = new FileInfo(sourceFileName);
                var dir = f.Directory.FullName;

                string thumbFolder = Path.Combine(dir, "thumb");
                if (!Directory.Exists(thumbFolder)) Directory.CreateDirectory(thumbFolder);
                string fileName = Path.Combine(thumbFolder, thumbKey);

                if (System.IO.File.Exists(fileName))
                    return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                else
                {
                    //一次只处理1个图片的缩略请求
                    lock (_lockObject)
                    {
                        if (System.IO.File.Exists(fileName))
                            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        else
                        {
                            //读取原图片，处理
                            var photo = PhotoFactory.Create(PathUtil.GetExtension(sourceFileName));
                            var format = sourceFileName.GetImageFormat();

                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (var source = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    switch (cutType)
                                    {
                                        case 1:
                                            if (format == ImageFormat.Jpeg)
                                            {
                                                using (var image = source.ToImage())
                                                {
                                                    using (var bitmap = image.Scale(width, height, ImageQuality.Height,Color.White))
                                                    {
                                                        bitmap.SaveAs(ms, format, 75);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                photo.ThumbByFill(source, ms, width, height, true);
                                            }
                                            break;
                                        case 2:
                                            photo.ThumbByCut(source, ms, width, height, true);
                                            break;
                                        case 3:
                                            photo.ThumbByFull(source, ms, width, height, true);
                                            break;
                                        case 4:
                                            photo.ThumbByPart(source, ms, width, height, true);
                                            break;
                                    }
                                }

                                ms.Seek(0, SeekOrigin.Begin);
                                var bytes = ms.GetBuffer();
                                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    fs.Write(bytes, 0, bytes.Length);
                                }
                            }

                            //输出文件
                            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        }
                    }
                }
            }
        }


        public Stream Load(string key)
        {
            var sourceFileName = ParsePath(key);

            return new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        #endregion

        public readonly static FileStorage Instance = new FileStorage();

    }
}
