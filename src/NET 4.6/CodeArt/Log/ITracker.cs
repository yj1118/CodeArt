using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Log
{
    /// <summary>
    /// 提供自定义的日志追踪内容
    /// </summary>
    public interface ITracker
    {
        string GetMessage();
    }
}
