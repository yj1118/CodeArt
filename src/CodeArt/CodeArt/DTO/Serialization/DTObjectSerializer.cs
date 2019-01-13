using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace CodeArt.DTO
{
    internal class DTObjectSerializer : IDTObjectSerializer
    {
        private DTObjectSerializer()
        {
        }

        public DTObject Serialize(object instance)
        {
            //if (instance == null) return DTObject.Create("{null}");   老代码，暂时保留
            if (instance == null) return DTObject.Empty;
            var instanceType = instance.GetType();
            if (instanceType == typeof(DTObject)) return (DTObject)instance;
            TypeMakupInfo typeInfo = TypeMakupInfo.GetTypeInfo(instanceType);
            return typeInfo.Serialize(instance);
        }

        public static readonly DTObjectSerializer Instance = new DTObjectSerializer();

    }
}

