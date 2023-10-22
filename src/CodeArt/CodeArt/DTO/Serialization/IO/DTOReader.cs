using System;
using System.IO;
using System.Runtime;

using System.Collections.Generic;
using System.Reflection;

namespace CodeArt.DTO
{
    internal abstract class DTOReader : IDTOReader
    {
        protected DTObject _dto;

        public DTOReader()
        {

        }


        public bool ReadBoolean(string name)
        {
            return _dto.GetValue<bool>(name, false);
        }

        public byte ReadByte(string name)
        {
            return _dto.GetValue<byte>(name, false);
        }


        public char ReadChar(string name)
        {
            return _dto.GetValue<char>(name, false);
        }


        public decimal ReadDecimal(string name)
        {
            return _dto.GetValue<decimal>(name, false);
        }

        public float ReadSingle(string name)
        {
            return _dto.GetValue<float>(name, false);
        }

        public double ReadDouble(string name)
        {
            return _dto.GetValue<double>(name, false);
        }

        public short ReadInt16(string name)
        {
            return _dto.GetValue<short>(name, false);
        }

        public int ReadInt32(string name)
        {
            return _dto.GetValue<int>(name, false);
        }

        public long ReadInt64(string name)
        {
            return _dto.GetValue<long>(name, false);
        }

        public string ReadString(string name)
        {
            return _dto.GetValue<string>(name, false);
        }


        public DateTime ReadDateTime(string name)
        {
            return _dto.GetValue<DateTime>(name, false);
        }

        public sbyte ReadSByte(string name)
        {
            return _dto.GetValue<sbyte>(name, false);
        }


        public ushort ReadUInt16(string name)
        {
            return _dto.GetValue<ushort>(name, false);
        }


        public uint ReadUInt32(string name)
        {
            return _dto.GetValue<uint>(name, false);
        }

        public ulong ReadUInt64(string name)
        {
            return _dto.GetValue<ulong>(name, false);
        }

        public Guid ReadGuid(string name)
        {
            return _dto.GetValue<Guid>(name, false);
        }

        public DateTime? ReadNullableDateTime(string name)
        {
            if (!_dto.Exist(name)) return null;
            return _dto.GetValue<DateTime>(name);
        }

        public byte? ReadNullableByte(string name)
        {
            if (!_dto.Exist(name)) return null;
            return _dto.GetValue<byte>(name);
        }

        public float? ReadNullableFloat(string name)
        {
            if (!_dto.Exist(name)) return null;
            return _dto.GetValue<float>(name);
        }

        public Guid? ReadNullableGuid(string name)
        {
            if (!_dto.Exist(name)) return null;
            return _dto.GetValue<Guid>(name);
        }

        public decimal? ReadNullableDecimal(string name)
        {
            if (!_dto.Exist(name)) return null;
            return _dto.GetValue<decimal>(name);
        }

        /// <summary>
        /// 读取数组长度
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int ReadLength(string name)
        {
            var dtoList = _dto.GetList(name, false);
            if (dtoList == null) return 0;
            return dtoList.Count;
        }

        public abstract T ReadElement<T>(string name, int index);

        public abstract T ReadObject<T>(string name);

    }
}
