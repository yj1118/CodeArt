using System;
using System.Collections.Generic;
using System.IO;

namespace CodeArt.DTO
{
    public interface IDTOReader
    {
        /// <summary>
        /// 从流中读取一个非空的布尔值
        /// </summary>
        /// <returns></returns>
        bool ReadBoolean(string name);

        /// <summary>
        /// 从流中读取一个字节
        /// </summary>
        /// <returns></returns>
        byte ReadByte(string name);


        /// <summary>
        /// 从流中读取一个非空的字符
        /// </summary>
        /// <returns></returns>
        char ReadChar(string name);


        /// <summary>
        /// 从流中读取DateTime对象，
        /// 该对象是由IPrimitiveWriter.Write(DateTime) 方法序列化的
        /// </summary>
        /// <returns></returns>
        DateTime ReadDateTime(string name);

        /// <summary>
        /// 从流中读取一个非空的 decimal 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        decimal ReadDecimal(string name);

        /// <summary>
        /// 从流中读取一个非空的 double 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        double ReadDouble(string name);

        /// <summary>
        /// 从流中读取一个非空的 Int16 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        short ReadInt16(string name);

        /// <summary>
        /// 从流中读取一个非空的 Int32 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        int ReadInt32(string name);

        /// <summary>
        /// 从流中读取一个非空的 Int64 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        long ReadInt64(string name);

        /// <summary>
        /// 从流中读取一个非空的 SByte 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        sbyte ReadSByte(string name);

        /// <summary>
        /// 从流中读取一个非空的 Single(浮点数) 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        float ReadSingle(string name);

        /// <summary>
        /// 从流中读取一个非空的 string 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        string ReadString(string name);

        /// <summary>
        /// 从流中读取一个非空的 UInt16 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        ushort ReadUInt16(string name);

        /// <summary>
        /// 从流中读取一个非空的 UInt32 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        uint ReadUInt32(string name);

        /// <summary>
        /// 从流中读取一个非空的 UInt64 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        ulong ReadUInt64(string name);

        /// <summary>
        /// 从流中读取一个非空的 Guid 值
        /// 请不要隐式转换
        /// 读的数据的类型就是它被写入时的类型
        /// 如果需要转换数据类型，请在读取数据之后进行转换
        /// </summary>
        /// <returns></returns>
        Guid ReadGuid(string name);


        DateTime? ReadNullableDateTime(string name);

        byte? ReadNullableByte(string name);

        float? ReadNullableFloat(string name);

        Guid? ReadNullableGuid(string name);

        decimal? ReadNullableDecimal(string name);


        /// <summary>
        /// 读取数组长度
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int ReadLength(string name);

        T ReadElement<T>(string name, int index);

        T ReadObject<T>(string name);

    }
}
