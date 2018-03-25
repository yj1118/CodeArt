using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;

namespace CodeArt.Web.WebPages
{
    public static class UploadRepositoryFactory
    {
        /// <summary>
        /// 注册上传文件存储器，请保证<paramref name="repository"/>是单例的
        /// </summary>
        /// <param name="locator"></param>
        public static void Register(IUploadRepository instance)
        {
            _instance = instance;
        }

        public static IUploadRepository Create()
        {
            return _instance;
        }

        private static IUploadRepository _instance = null;

    }
}
