using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.TestTools
{
    public enum LogContentType : byte
    {
        /// <summary>
        /// 表示开始运行测试
        /// </summary>
        Start = 1,
        /// <summary>
        /// 表示结束测试
        /// </summary>
        End=2,
        /// <summary>
        /// 表示测试方法运行的情况
        /// </summary>
        Run = 3
    }

}
