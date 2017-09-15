using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using CodeArt.Util;

namespace CodeArt.DTO
{
    /// <summary>
    /// 封装了一组常量，这些数值会在动态生成代码时使用
    /// </summary>
    internal static class SerializationArgs
    {
        /// <summary>
        /// instance在serialize方法中的参数索引
        /// </summary>
        public const int InstanceIndex = 0;
        /// <summary>
        /// writer在serialize方法中的参数索引
        /// </summary>
        public const int WriterIndex = 1;
        /// <summary>
        /// serializer在serialize方法中的参数索引
        /// </summary>
        public const int SerializerIndex = 2;

        public const int SerializerTypeNameTableIndex = 3;

        public const int ReaderIndex = 1;
        public const int DeserializerIndex = 1;
        public const int DeserializerTypeNameTableIndex = 2;

        public const string InstanceName = "instance";

        public const string VersionName = "version";


        public const string StreamPosition = "streamPosition";
        public const string StreamLength = "streamLength";

        //public const string TypeNameTable = "typeNameTable";

    }
}