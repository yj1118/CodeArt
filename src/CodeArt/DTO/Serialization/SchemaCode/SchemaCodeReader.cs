using System;
using System.IO;
using System.Runtime;

using System.Collections.Generic;
using System.Reflection;

namespace CodeArt.DTO
{
    internal class SchemaCodeReader : DTOReader
    {
        public SchemaCodeReader()
        {
        }

        /// <summary>
        /// 成员的架构代码
        /// </summary>
        private SchemaCodes _schemaCodes;



        public void Initialize(DTObject dto, SchemaCodes schemaCodes)
        {
            _dto = dto;
            _schemaCodes = schemaCodes;
        }

        public void Reset()
        {
            _dto = null;
            _schemaCodes = null;
        }

        public override T ReadElement<T>(string name, int index)
        {
            var dtoList = _dto.GetList(name, false);
            if (dtoList == null) return default(T);
            var dtoElement = dtoList[index];
            if (dtoElement.IsSingleValue) return dtoElement.GetValue<T>();

            var elementType = typeof(T);
            string schemaCode = _schemaCodes.GetSchemaCode(name, () => elementType);
            return (T)DTObjectMapper.Instance.Save(elementType, schemaCode, dtoElement);
        }

        public override T ReadObject<T>(string name)
        {
            var dtoValue = _dto.GetObject(name, false);
            if (dtoValue == null) return default(T);

            var objType = typeof(T);
            string schemaCode = _schemaCodes.GetSchemaCode(name,()=> objType);
            return (T)DTObjectMapper.Instance.Save(objType, schemaCode, dtoValue);
        }

    }
}
