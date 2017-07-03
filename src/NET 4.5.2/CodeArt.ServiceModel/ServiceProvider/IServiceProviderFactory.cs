using System.ServiceModel;
using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 服务提供者工厂
    /// </summary>
    public interface IServiceProviderFactory
    {
        IServiceProvider Create(ServiceRequest request);
    }
}
