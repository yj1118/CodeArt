using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

using CodeArt.Util;

namespace CodeArt.DTO
{
    internal class DTObjectMapper : IDTObjectMapper
    {
        private DTObjectMapper()
        {
        }


        /// <summary>
        /// 根据架构代码，将dto的数据创建到新实例<paramref name="instanceType"/>中
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public object Save(Type instanceType, string schemaCode, DTObject dto)
        {
            if (instanceType == typeof(DTObject)) return dto.Clone();
            TypeSchemaCodeInfo typeInfo = TypeSchemaCodeInfo.GetTypeInfo(instanceType, schemaCode);
            return typeInfo.Deserialize(dto);
        }

        /// <summary>
        /// 根据架构代码，将dto的数据写入到新实例<paramref name="instanceType"/>中
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="schemaCode"></param>
        /// <param name="dto"></param>
        public void Save(object instance, string schemaCode, DTObject dto)
        {
            //if (instance.IsNull()) return;
            var instanceType = instance.GetType();
            if (instanceType == typeof(DTObject)) instance = dto.Clone();
            TypeSchemaCodeInfo typeInfo = TypeSchemaCodeInfo.GetTypeInfo(instanceType, schemaCode);
            typeInfo.Deserialize(instance, dto);
        }

        /// <summary>
        /// 根据架构代码将对象的信息创建dto
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public DTObject Load(string schemaCode, object instance)
        {
            if (instance.IsNull()) return DTObject.Empty;
            var instanceType = instance.GetType();
            if (instanceType == typeof(DTObject)) return (DTObject)instance;
            TypeSchemaCodeInfo typeInfo = TypeSchemaCodeInfo.GetTypeInfo(instanceType, schemaCode);
            return typeInfo.Serialize(instance);
        }

        /// <summary>
        /// 根据架构代码将对象的信息加载到dto中
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="schemaCode"></param>
        /// <param name="instance"></param>
        public void Load(DTObject dto, string schemaCode, object instance)
        {
            if (instance.IsNull()) return;
            var instanceType = instance.GetType();
            if (instanceType == typeof(DTObject)) return;
            TypeSchemaCodeInfo typeInfo = TypeSchemaCodeInfo.GetTypeInfo(instanceType, schemaCode);
            typeInfo.Serialize(instance, dto);
        }


        public static readonly DTObjectMapper Instance = new DTObjectMapper();

    }
}

