using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Module.Minio
{
    /// <summary>
    /// 
    /// </summary>
    public static class TemporaryStorage
    {
        private static string _bucket;
        private static MinioHelper _minio = null;

        static TemporaryStorage()
        {
            const string configBucket = "minio_bucket";
            _bucket = ConfigurationManager.AppSettings[configBucket];
            if (_bucket == null) throw new ApplicationException(string.Format("NotSetupMinio", configBucket));

            _minio = new MinioHelper();
        }

        private static string GetTempFile(string tempKey)
        {
            //return FileStorage.GetPath(tempKey, "temp");
            return Path.Combine(_bucket, "temp", tempKey);
        }

        public static string Begin(string extension)
        {
            // 创建存储桶
            Task<bool> maked = _minio.MakeBucket(_bucket);
            maked.Wait();
            if (!maked.Result) throw new ApplicationException("CreateBucketFail");

            string tempKey = string.Format("{0}.{1}", Guid.NewGuid().ToString("n"), extension);
            //string fileName = GetTempFile(tempKey);
            //using (FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
            //{

            //}
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

            using (var ms = new MemoryStream())
            {
                ms.Write(buffer, offset, count);
                //Task<bool> success = _minio.PutObjectAsync(_bucket, tempKey, ms, count);
                //success.Wait();
                //if (!success.Result) throw new ApplicationException(Strings.TempFileWriteFail);
            }

            //using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            //{
            //    fs.Write(buffer, offset, count);
            //}
        }

        /// <summary>
        /// 结束写入操作
        /// </summary>
        /// <param name="tempKey">临时文件的编号</param>
        /// <returns>返回文件唯一标识符</returns>
        public static string End(string tempKey)
        {
            string tempFileName = GetTempFile(tempKey);
            return FileStorage.Instance.MoveAndCompress(tempFileName);
        }

        /// <summary>
        /// 结束写入操作，并以指定的key存储文件
        /// </summary>
        /// <param name="tempKey">临时文件的编号</param>
        /// <returns>true:替换了文件，false:新增的文件</returns>
        public static void End(string tempKey,string fileKey)
        {
            string tempFileName = GetTempFile(tempKey);
            FileStorage.Instance.MoveAndCompress(tempFileName, fileKey);
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
