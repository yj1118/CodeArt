using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace CodeArt.DTO
{
    internal class DTObjectDeserializer : IDTObjectDeserializer
    {
        private DTObjectDeserializer()
        {
        }

        public object Deserialize(Type objectType, DTObject dto)
        {
            if (objectType == typeof(DTObject)) return dto;
            TypeMakupInfo typeInfo = TypeMakupInfo.GetTypeInfo(objectType);
            return typeInfo.Deserialize(dto);
        }

        /// <summary>
        /// 将dto的内容反序列化到<paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="dto"></param>
        public void Deserialize(object instance, DTObject dto)
        {
            TypeMakupInfo typeInfo = TypeMakupInfo.GetTypeInfo(instance.GetType());
            typeInfo.Deserialize(instance, dto);
        }

        public static readonly DTObjectDeserializer Instance = new DTObjectDeserializer();

    }
}

