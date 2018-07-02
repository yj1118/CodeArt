using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 元数据类型，记录了对象拥有的领域属性等数据
    /// </summary>
    internal class RuntimeObjectType : RuntimeType
    {
        /// <summary>
        /// 类型定义对应的实例类型（也就是真正需要构建的对象类型）
        /// </summary>
        public Type ObjectType
        {
            get
            {
                return this.Define.ObjectType;
            }
        }

        public TypeDefine Define
        {
            get;
            private set;
        }


        public RuntimeObjectType(string name, TypeDefine define)
            : base(name)
        {
            this.Define = define;
        }

        /// <summary>
        /// 识别类型是否为运行时的类型，返回真正的领域对象的类型
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static Type GetDomainType(Type objectType)
        {
            var runtimeType = objectType as RuntimeObjectType;
            return runtimeType == null ? objectType : runtimeType.ObjectType;
        }


    }
}
