using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示对象具备日志的能力
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ObjectLogableAttribute : Attribute
    {
        /// <summary>
        /// 对象在日志里的命名空间
        /// </summary>
        public string TypeNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// 对象在日志里的类型名称
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 架构代码，决定需要存储的与对象自身有关的信息
        /// </summary>
        public string SchemaCode
        {
            get;
            private set;
        }


        internal Type ObjectType
        {
            get;
            private set;
        }

        private string _typeFullName;

        public string TypeFullName
        {
            get
            {
                if (string.IsNullOrEmpty(_typeFullName))
                {
                    _typeFullName = string.Format("{0}{1}",
                                                    string.IsNullOrEmpty(this.TypeNamespace) ? string.Empty : string.Format("{0}:", this.TypeNamespace),
                                                    string.IsNullOrEmpty(this.TypeName) ? this.ObjectType.Name : this.TypeName);
                }
                return _typeFullName;
            }
        }


        /// <summary>
        /// 架构代码，该代码会决定那些属性会参与日志的存储
        /// </summary>
        /// <param name="schemaCode"></param>
        public ObjectLogableAttribute(string schemaCode)
        {
            this.SchemaCode = schemaCode;
        }


        /// <summary>
        /// 在程序启动的时候分析所有日志特性，记录信息
        /// </summary>
        internal static void Initialize()
        {
            var types = DomainObject.TypeIndex;

            foreach (var objectType in types)
            {
                if (DomainObject.IsEmpty(objectType)) continue;

                var tip = Create(objectType);
                if (tip != null)
                {
                    _tips.Add(tip);
                }
            }
        }


        private static ObjectLogableAttribute Create(Type objectType)
        {
            var tip = AttributeUtil.GetAttribute<ObjectLogableAttribute>(objectType);

            if (tip != null)
            {
                string typeNamespace = string.Empty;
                string typeName = objectType.Name;

                if (!string.IsNullOrEmpty(tip.TypeNamespace)) typeNamespace = tip.TypeNamespace;
                if (!string.IsNullOrEmpty(tip.TypeName)) typeName = tip.TypeName;

                tip.TypeNamespace = typeNamespace;  //重新赋值一次
                tip.TypeName = typeName;//重新赋值一次
                tip.ObjectType = objectType;
            }
            return tip;
        }


        private readonly static List<ObjectLogableAttribute> _tips = new List<ObjectLogableAttribute>();

        /// <summary>
        /// 获得当前应用程序定义的所有具备日志能力的特性标签
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ObjectLogableAttribute> GetTips()
        {
            return _tips;
        }

        public static ObjectLogableAttribute GetTip(Type objectType)
        {
            return _getTip(objectType);
        }

        private static Func<Type, ObjectLogableAttribute> _getTip
                = LazyIndexer.Init<Type, ObjectLogableAttribute>((objectType) =>
                {
                    var result = _tips.FirstOrDefault((tip) => tip.ObjectType == objectType);
                    return result;
                });
    }
}
