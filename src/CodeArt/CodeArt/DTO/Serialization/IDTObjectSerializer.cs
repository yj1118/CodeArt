using System;
using System.IO;
using System.Collections.Generic;

namespace CodeArt.DTO
{
    /// <summary>
    /// 对象序列化器
    /// </summary>
    public interface IDTObjectSerializer
    {
        /// <summary>
        /// 将对象实例的信息序列化到dto中
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        DTObject Serialize(object instance);
    }
}

