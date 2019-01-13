using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DTO
{
    internal class SchemaCodeWriter : DTOWriter
    { 
        public SchemaCodeWriter()
        {
        }

        /// <summary>
        /// 架构代码
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

        public override void WriteElement<T>(string name, bool elementIsPrimitive, T element)
        {
            if (elementIsPrimitive)
            {
                var elementDTO = _dto.CreateAndPush(name);
                elementDTO.SetValue(element);
            }
            else
            {
                var elementType = typeof(T);
                if(elementType.IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(elementType);
                    var value = DataUtil.ToValue(element, underlyingType);
                    var elementDTO = _dto.CreateAndPush(name);
                    elementDTO.SetValue(value);
                }
                else
                {
                    string schemaCode = _schemaCodes.GetSchemaCode(name, () => typeof(T));
                    var elementDTO = DTObjectMapper.Instance.Load(schemaCode, element);
                    _dto.Push(name, elementDTO);
                }
            }
        }


        public override void Write(string name, object value)
        {
            if (value.IsNull()) return; //为isNull的成员不输出
            //是否自定义
            var serializable = value as IDTOSerializable;
            if (serializable != null)
            {
                serializable.Serialize(_dto, name);
                return;
            }
            string schemaCode = _schemaCodes.GetSchemaCode(name, () => value.GetType());
            var dtoValue = DTObjectMapper.Instance.Load(schemaCode, value);
            _dto.SetObject(name, dtoValue);
        }
    }
}
