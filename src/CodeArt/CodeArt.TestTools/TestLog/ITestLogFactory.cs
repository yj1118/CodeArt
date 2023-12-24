using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 服务提供者工厂
    /// </summary>
    public interface ITestLogFactory
    {
        ITestLog Create();
    }
}
