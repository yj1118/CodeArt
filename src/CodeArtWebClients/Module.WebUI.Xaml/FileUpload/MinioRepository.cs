﻿using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.ServiceModel;
using CodeArt.Runtime;
using CodeArt.Web.WebPages;
using CodeArt.Web;

using Module.File.Minio;

namespace Module.WebUI.Xaml
{
    /// <summary>
    /// Minio的文件存储器
    /// </summary>
    public sealed class MinioRepository : IUploadRepository
    {
        private MinioRepository() { }


        public string Begin(string extension)
        {
            return TemporaryStorage.Begin(extension);
        }


        public void Write(string tempKey, byte[] buffer, int offset, int count)
        {
            TemporaryStorage.Write(tempKey, buffer, offset, count);
        }


        public string Close(string tempKey)
        {
            return TemporaryStorage.End(tempKey);
        }

        public void Delete(string tempKey)
        {
        }

        public readonly static MinioRepository Instance = new MinioRepository();

    }
}
