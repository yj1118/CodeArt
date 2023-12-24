using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.TestPlatform
{
    public enum CleanupLevel : byte
    {
        /// <summary>
        /// 不自动清理
        /// </summary>
        None = 1,
        /// <summary>
        /// 在场景的开始和末尾要清理数据
        /// </summary>
        Scene = 2,
        /// <summary>
        /// 在测试用例的开始和末尾要清理数据
        /// </summary>
        Case = 3
    }
}

