using System;
using System.IO;
using System.Collections.Generic;

namespace CodeArt.DTO
{
    /// <summary>
    /// 对象序列化器
    /// </summary>
    public interface IDTObjectDeserializer
    {
        /// <summary>
        /// 将dto的信息反序列化到对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        object Deserialize(Type objectType, DTObject dto);
    }
}

