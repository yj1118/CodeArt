using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using System.Collections.Generic;
using CodeArt.Runtime;

namespace CodeArt.DTO
{
    internal class MarkupWriter : DTOWriter
    { 
        public MarkupWriter()
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


        public override void WriteElement<T>(string name, bool elementIsPrimitive, T element)
        {
            if (elementIsPrimitive)
            {
                var elementDTO = _dto.CreateAndPush(name);
                elementDTO.SetValue(element);
            }
            else
            {
                var elementDTO = DTObjectSerializer.Instance.Serialize(element, _dto.IsPinned);
                _dto.Push(name, elementDTO);
            }

        }

        public override void Write(string name, object value)
        {
            var dtoValue = DTObjectSerializer.Instance.Serialize(value, _dto.IsPinned);
            _dto.SetObject(name, dtoValue);
        }
    }
}
