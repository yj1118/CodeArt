using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.TestCoder.Subsystem
{
    /// <summary>
    /// 命令参数的类型，是来自构造函数参数还是属性
    /// </summary>
    public enum CommandInputType : byte
    {
        /// <summary>
        /// 构造函数参数
        /// </summary>
        ConstructorParameter = 1,
        /// <summary>
        /// 属性
        /// </summary>
        Property = 2
    }
}
