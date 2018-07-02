using System;
using System.Configuration;
using System.IO;

using CodeArt.Util;

namespace Module.File
{
    /// <summary>
    /// 
    /// </summary>
    public static class TemporaryStorage
    {

        //#region 接受的参数

        private static string GetTempFile(string tempKey)
        {
            const string configName = "fileStorage";
            string folder = ConfigurationManager.AppSettings[configName];
            if (folder == null) throw new ApplicationException(string.Format(Strings.CannotWriteTemporaryFile, configName));
            folder = Path.Combine(folder, "temp");

            string code = tempKey.MD5();
            string balancedFolder = code.Substring(0, 8).Replace("-", "\\");
            balancedFolder = Path.Combine(folder, balancedFolder);
            balancedFolder = Path.Combine(balancedFolder, code);

            if (!Directory.Exists(balancedFolder)) Directory.CreateDirectory(balancedFolder);
            return Path.Combine(balancedFolder, tempKey);
        }

        public static string Begin(string extension)
        {
            string tempKey = string.Format("{0}.{1}", Guid.NewGuid().ToString("n"), extension);
            string fileName = GetTempFile(tempKey);
            using (FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
            {

            }
            return tempKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempKey">临时文件的编号</param>
        /// <param name="content">需要写入的内容</param>
        /// <returns></returns>
        public static void Write(string tempKey, byte[] buffer, int offset, int count)
        {
            string fileName = GetTempFile(tempKey);
            using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Write(buffer, offset, count);
            }
        }

        /// <summary>
        /// 结束写入操作
        /// </summary>
        /// <param name="tempKey">临时文件的编号</param>
        /// <returns>返回文件唯一标识符</returns>
        public static string End(string tempKey)
        {
            string tempFileName = GetTempFile(tempKey);
            return FileStorage.Instance.Move(tempFileName);
        }

        /// <summary>
        /// 结束写入操作，并以指定的key存储文件
        /// </summary>
        /// <param name="tempKey">临时文件的编号</param>
        /// <returns>true:替换了文件，false:新增的文件</returns>
        public static void End(string tempKey,string fileKey)
        {
            string tempFileName = GetTempFile(tempKey);
            FileStorage.Instance.Move(tempFileName, fileKey);
        }
      
        public static void Delete(string key)
        {
            FileStorage.Instance.Delete(key);
        }

        public static void DeleteTemp(string tempKey)
        {
            string tempFileName = GetTempFile(tempKey);
            if (System.IO.File.Exists(tempFileName))
                System.IO.File.Delete(tempFileName);
        }

    }
}
