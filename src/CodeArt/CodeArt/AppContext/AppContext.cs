using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

using CodeArt.DTO;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt
{
    public static class AppContext
    {
        /// <summary>
        /// 引用程序目录
        /// </summary>
        public static string ProcessDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string ProcessDirectoryParent
        {
            get;
            private set;
        }


        public static bool IsX64
        {
            get
            {
                return (Marshal.SizeOf<IntPtr>() == sizeof(long));
            }
        }

        /// <summary>
        /// 将相对路径映射为绝对路径，如果参数<paramref name="path"/>是绝对路径那么直接返回原值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string path)
        {
            if (path.IndexOf("\\") > -1) return path;
            if (path.StartsWith("/")) path = path.Substring(1);
            var root = ProcessDirectory;
            if (path.StartsWith("../"))
            {
                root = ProcessDirectoryParent;
                path = path.Substring(3);
            }
            path = path.Replace("/", "\\");
            return Path.Combine(root, path);
        }


        #region 语言和身份

        public static string LocalIdentityName
        {
            get
            {
                return LocalIdentity.GetValue<string>("name", string.Empty);
            }
        }

        public static string LocalLanguage
        {
            get
            {
                return LocalIdentity.GetValue<string>("language", string.Empty);
            }
        }

        /// <summary>
        /// 系统本地的身份（也就是通过配置文件配置的身份）
        /// </summary>
        public readonly static DTObject LocalIdentity;

        private static DTObject GetLocalIdentity()
        {
            var identity = DTObject.Create();
            identity.SetValue("name", CodeArt.Configuration.Current.IdentityName);
            identity.SetValue("language", CodeArt.Configuration.Current.Language);
            return identity.AsReadOnly();
        }

        /// <summary>
        /// 获得应用程序的身份，当会话身份存在时使用会话身份，否则使用本地身份
        /// </summary>
        public static DTObject Identity
        {
            get
            {
                return AppSession.Identity ?? LocalIdentity;
            }
        }

        #endregion


        static AppContext()
        {
            LocalIdentity = GetLocalIdentity();
            ProcessDirectoryParent = new DirectoryInfo(ProcessDirectory).Parent.FullName;
        }

    }
}
