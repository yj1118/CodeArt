using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.Web.Mobile
{
    /// <summary>
    /// 服务提供者工厂
    /// </summary>
    public interface IProcedureFactory
    {
        IProcedure Create(string virtualPath);
    }
}
