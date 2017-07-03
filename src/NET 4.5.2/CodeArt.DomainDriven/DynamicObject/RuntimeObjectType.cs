using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    internal class RuntimeObjectType : RuntimeType
    {
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
