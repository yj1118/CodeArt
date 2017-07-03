using System;
using System.Linq;

namespace CodeArt.Runtime.IL
{
    internal enum ItemType
    {
        /// <summary>
        /// 值类型
        /// </summary>
        Value,
        /// <summary>
        /// 引用类型
        /// </summary>
        Reference,
        /// <summary>
        /// 值类型的指针
        /// </summary>
        AddressToValue,
        /// <summary>
        /// 引用类型的指针
        /// </summary>
        AddressToReference
    }
}
