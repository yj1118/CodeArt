using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    internal static class RemoteActionName
    {
        public static string GetObject(RemoteType type)
        {
            return _getObject(type);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        private static Func<RemoteType, string> _getObject = LazyIndexer.Init<RemoteType, string>((type) =>
        {
            if (string.IsNullOrEmpty(type.Namespace)) return string.Format("d:Get{0}", type.Name);
            return string.Format("d.{0}:Get{1}", type.Namespace, type.Name);
        });


        /// <summary>
        /// 对象已更新
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ObjectUpdated(RemoteType type)
        {
            return _getObjectUpdated(type);
        }

        private static Func<RemoteType, string> _getObjectUpdated = LazyIndexer.Init<RemoteType, string>((type) =>
        {
            if (string.IsNullOrEmpty(type.Namespace)) return string.Format("d:{0}Updated", type.Name);
            return string.Format("d.{0}:{1}Updated", type.Namespace, type.Name);
        });


        /// <summary>
        /// 对象已删除
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ObjectDeleted(RemoteType type)
        {
            return _getObjectDeleted(type);
        }

        private static Func<RemoteType, string> _getObjectDeleted = LazyIndexer.Init<RemoteType, string>((type) =>
        {
            if (string.IsNullOrEmpty(type.Namespace)) return string.Format("d:{0}Deleted", type.Name);
            return string.Format("d.{0}:{1}Deleted", type.Namespace, type.Name);
        });
    }
}
