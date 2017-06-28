using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.DomainDriven
{
    public enum PropertyChangedMode : byte
    {
        /// <summary>
        /// 需要比较，指示需要将属性的新老值对比，判断是否真的更改
        /// </summary>
        Compare = 1,
        /// <summary>
        /// 确定的，意味着只要对属性赋值，那么属性必然是被更改的
        /// 这一般用于大型对象的更改，以便提高性能
        /// </summary>
        Definite
    }
}