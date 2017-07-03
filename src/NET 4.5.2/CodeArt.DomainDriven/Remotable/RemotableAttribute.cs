using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示对象具备远程的能力
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class RemotableAttribute : Attribute
    {
        /// <summary>
        /// 对象的远程命名空间
        /// </summary>
        public string TypeNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// 对象的远程类型名称
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 架构代码，该代码会决定那些属性会参与远程数据的访问
        /// </summary>
        public string SchemaCode
        {
            get;
            private set;
        }

        internal RemoteType RemoteType
        {
            get;
            private set;
        }

        internal Type ObjectType
        {
            get;
            private set;
        }


        /// <summary>
        /// 架构代码，该代码会决定那些属性会参与远程数据的访问
        /// </summary>
        /// <param name="schemaCode"></param>
        public RemotableAttribute(string schemaCode)
        {
            this.SchemaCode = schemaCode;
        }



        /// <summary>
        /// 在程序启动的时候分析所有远程特性，记录信息
        /// </summary>
        internal static void Initialize()
        {
            var types = DomainObject.TypeIndex;
            foreach(var objectType in types)
            {
                var tip = Create(objectType);
                if(tip != null)
                {
                    _tips.Add(tip);
                }
            }
        }

        /// <summary>
        /// 创建并补全<paramref name="objectType"/>定义的远程能力特性，特性会被加到索引表
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private static RemotableAttribute Create(Type objectType)
        {
            var tip = AttributeUtil.GetAttribute<RemotableAttribute>(objectType);
            if (tip != null)
            {
                string typeNamespace = string.Empty;
                string typeName = objectType.Name;

                if (!string.IsNullOrEmpty(tip.TypeNamespace)) typeNamespace = tip.TypeNamespace;
                if (!string.IsNullOrEmpty(tip.TypeName)) typeName = tip.TypeName;

                tip.TypeNamespace = typeNamespace;  //重新赋值一次
                tip.TypeName = typeName;//重新赋值一次
                tip.RemoteType = new RemoteType(typeNamespace, typeName);
                tip.ObjectType = objectType;
            }
            return tip;
        }


        private readonly static List<RemotableAttribute> _tips = new List<RemotableAttribute>();

        public static RemotableAttribute GetTip(Type objectType)
        {
            return _tips.FirstOrDefault((tip) => tip.ObjectType == objectType);
        }


        public static RemotableAttribute GetTip(string typeFullName)
        {
            return _getTip(typeFullName);
        }

        private static Func<string, RemotableAttribute> _getTip
                = LazyIndexer.Init<string, RemotableAttribute>((typeFullName) =>
                {
                    var result = _tips.FirstOrDefault((tip) => tip.RemoteType.FullName.EqualsIgnoreCase(typeFullName));
                    if (result == null)
                        throw new DomainDrivenException(string.Format(Strings.NotFoundRemotable, typeFullName));
                    return result;
                });
    }
}
