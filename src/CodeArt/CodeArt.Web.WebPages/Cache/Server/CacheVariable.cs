using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using CodeArt.Util;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 缓存的环境变量
    /// </summary>
    public class CacheVariable
    {
        /// <summary>
        /// 页面地址
        /// </summary>
        public string Url
        {
            get;
            private set;
        }

        /// <summary>
        /// 压缩方式 
        /// </summary>
        public HttpCompressionType CompressionType
        {
            get;
            private set;
        }

        public ClientDevice Device
        {
            get;
            private set;
        }

        public CacheVariable(string url, HttpCompressionType compressionType, ClientDevice device)
        {
            this.Url = url;
            this.CompressionType = compressionType;
            this.Device = device;
        }

        private string _uniqueCode = null;
        public string UniqueCode
        {
            get
            {
                if (_uniqueCode == null) _uniqueCode = GetUniqueCode();
                return _uniqueCode;
            }
        }

        private string GetDeviceType(ClientDevice device)
        {
            return device.IsMobile ? "mobile" : "pc";
        }

        /// <summary>
        /// 获取缓存变量的唯一码，这常用于缓存存储
        /// </summary>
        /// <returns></returns>
        protected virtual string GetUniqueCode()
        {
            return string.Format("{0}_{1}_{2}", this.Url, (byte)this.CompressionType, GetDeviceType(this.Device)).MD5();
        }

    }
}