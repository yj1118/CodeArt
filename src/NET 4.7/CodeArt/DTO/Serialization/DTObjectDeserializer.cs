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

        public T Deserialize<T>(DTObject obj)
        {
            var targetType = typeof(T);
            if (targetType == typeof(DTObject)) return (T)((object)obj);
            TypeMakupInfo typeInfo = TypeMakupInfo.GetTypeInfo(targetType);
            return (T)typeInfo.Deserialize(obj);
        }

        public static readonly DTObjectDeserializer Instance = new DTObjectDeserializer();

    }
}

