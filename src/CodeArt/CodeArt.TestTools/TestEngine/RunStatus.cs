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
    public enum RunStatus : byte
    {
        None = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 报错
        /// </summary>
        Error = 2,
        /// <summary>
        /// 进行中
        /// </summary>
        Ing=3,
        /// <summary>
        /// 被停止
        /// </summary>
        Stop = 4
    }

}
