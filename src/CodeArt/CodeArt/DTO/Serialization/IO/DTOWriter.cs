using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using System.Collections.Generic;
using CodeArt.Runtime;

namespace CodeArt.DTO
{
    internal abstract class DTOWriter : IDTOWriter
    { 
        protected DTObject _dto;

        public DTOWriter()
        {
        }

      

        public void Write(string name, DateTime value)
        {
            _dto.SetValue(name, value);
        }

        public void Write(string name, DateTime? value)
        {
            if(value.HasValue) _dto.SetValue(name, value.Value);
        }

        public void Write(string name, byte? value)
        {
            if (value.HasValue) _dto.SetValue(name, value.Value);
        }

        public void Write(string name, float? value)
        {
            if (value.HasValue) _dto.SetValue(name, value.Value);
        }

        public void Write(string name, decimal? value)
        {
            if (value.HasValue) _dto.SetValue(name, value.Value);
        }

        public void Write(string name, Guid? value)
        {
            if (value.HasValue) _dto.SetValue(name, value.Value);
        }

        public void Write(string name, string value)
        {
            _dto.SetValue(name, value);
        }


        public void Write(string name, float value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, double value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, uint value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, ulong value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, ushort value)
        {
            _dto.SetValue(name, value);
        }

        public void Write(string name, sbyte value)
        {
            _dto.SetValue(name, value);
        }

        public void Write(string name, char value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, byte value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, bool value)
        {
            _dto.SetValue(name, value);
        }

        public void Write(string name, decimal value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, long value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, short value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, int value)
        {
            _dto.SetValue(name, value);
        }
        public void Write(string name, Guid value)
        {
            _dto.SetValue(name, value);
        }

        public abstract void WriteElement<T>(string name, bool elementIsPrimitive, T element);

        public abstract void Write(string name, object value);

        public void WriteArray(string name)
        {
            _dto.SetList(name);
        }
    }
}
