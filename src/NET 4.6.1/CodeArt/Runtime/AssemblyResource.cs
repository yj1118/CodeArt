using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using CodeArt.IO;

namespace CodeArt.Runtime
{
    public static class AssemblyResource
    {
        /// <summary>
        /// 加载文本文件资源
        /// </summary>
        /// <returns></returns>
        public static string LoadText(Assembly assembly, string resourceName)
        {
            string text = string.Empty;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;
                using (StreamReader reader = new StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            return text;
        }

        /// <summary>
        /// 加载文本文件资源
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string LoadText(string resourceKey)
        {
            var temp = resourceKey.Split(',');
            var resourceName = temp[0];
            var assemblyName = temp[1];
            return AssemblyResource.LoadText(Assembly.Load(assemblyName), resourceName);
        }

        public static byte[] LoadBytes(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;
                byte[] buffer = new byte[stream.Length];
                stream.ReadPro(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        public static byte[] LoadBytes(string resourceKey)
        {
            var temp = resourceKey.Split(',');
            var resourceName = temp[0];
            var assemblyName = temp[1];
            return AssemblyResource.LoadBytes(Assembly.Load(assemblyName), resourceName);
        }

        ///// <summary>
        ///// 加载文本文件资源
        ///// </summary>
        ///// <returns></returns>
        //public static string LoadText(string assemblyFile, string resourceName)
        //{
        //    var assembly = Assembly.LoadFrom(assemblyFile);
        //    return LoadText(assembly, resourceName);
        //}
    }
}
