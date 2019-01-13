using System;
using System.IO;
using System.Collections.Generic;

namespace CodeArt.DTO
{
    /// <summary>
    /// dto对象映射器
    /// </summary>
    internal interface IDTObjectMapper
    {
        /// <summary>
        /// 根据架构代码，将dto的数据创建到新实例<paramref name="instanceType"/>中
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        object Save(Type instanceType, string schemaCode, DTObject dto);

        /// <summary>
        /// 根据架构代码，将dto的数据写入到新实例<paramref name="instanceType"/>中
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="schemaCode"></param>
        /// <param name="dto"></param>
        void Save(object instance, string schemaCode, DTObject dto);


        /// <summary>
        /// 根据架构代码将对象的信息创建dto
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        DTObject Load(string schemaCode, object instance);

        /// <summary>
        /// 根据架构代码将对象的信息加载到dto中
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        void Load(DTObject dto, string schemaCode, object instance);

    }
}

