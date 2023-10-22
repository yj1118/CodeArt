using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.TestPlatform
{
    /// <summary>
    /// 服务录制器工厂
    /// </summary>
    public interface IVariableProviderFactory
    {
        IVariableProvider Create(string package, string name);
    }
}