using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
namespace CodeArt.Runtime
{
    public static class TypeUtil
    {
        /// <summary>
        /// 创建实例，typeName会被缓存
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static object CreateInstance(string typeName)
        {
            Type instanceType = GetType(typeName);
            return instanceType == null ? null : instanceType.CreateInstance();
        }

        public static Type GetType(string typeName)
        {
            return _getType(typeName);
        }

        private static Func<string, Type> _getType = LazyIndexer.Init<string, Type>((typeName) =>
        {
            return Type.GetType(typeName);
        });

        /// <summary>
        /// 获得泛型的类型
        /// </summary>
        /// <param name="genericName"></param>
        /// <param name="memberNames"></param>
        /// <returns></returns>
        public static Type GetGenericType(Type genericBaseType, params Type[] memberTypes)
        {
            var genericName = GetGenericBaseFullName(genericBaseType);
            StringBuilder typeName = new StringBuilder();
            typeName.Append(genericName);
            typeName.Append("[");
            foreach (var memberType in memberTypes)
            {
                typeName.AppendFormat("[{0}],", memberType.AssemblyQualifiedName);
            }
            if (memberTypes.Length > 0) typeName.Length--;
            typeName.Append("]");
            return Type.GetType(typeName.ToString());
        }

        private static string GetGenericBaseFullName(Type genericBaseType)
        {
            return genericBaseType.FullName;
        }

    }
}
