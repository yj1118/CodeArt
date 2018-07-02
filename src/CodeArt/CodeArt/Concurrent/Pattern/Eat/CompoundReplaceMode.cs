using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

using System.Threading;
using CodeArt.IO;


namespace CodeArt.Concurrent.Pattern
{
    /// <summary>
    /// 复合物的替换模式
    /// </summary>
    public enum CompoundReplaceMode
    {
        /// <summary>
        /// 喜新，当复合物达到上限，会抛弃旧的复合物，加入新复合物
        /// </summary>
        HappyNew,
        /// <summary>
        /// 喜旧，当复合物达到上限，不再接受新的复合物
        /// </summary>
        HappyOld
    }
}
