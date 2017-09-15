using System;
using System.IO;
using System.Runtime;

using System.Collections.Generic;
using System.Reflection;

namespace CodeArt.DTO
{
    internal class MarkupReader : DTOReader
    {
        public MarkupReader()
        {
        }

        public void Initialize(DTObject dto)
        {
            _dto = dto;
        }

        public void Reset()
        {
            _dto = null;
        }


        public override T ReadElement<T>(string name, int index)
        {
            var dtoList = _dto.GetList(name, false);
            if (dtoList == null) return default(T);
            var dtoElement = dtoList[index];
            if (dtoElement.IsSingleValue) return dtoElement.GetValue<T>();
            return (T)DTObjectDeserializer.Instance.Deserialize(typeof(T), dtoElement);
        }


        public override T ReadObject<T>(string name)
        {
            var dtoValue = _dto.GetObject(name);
            if (dtoValue == null) return default(T);
            return (T)DTObjectDeserializer.Instance.Deserialize(typeof(T), dtoValue);
        }

    }
}
