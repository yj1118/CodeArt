using System.ServiceModel;
using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 服务录制器工厂
    /// </summary>
    public interface IServiceRecorderFactory
    {
        IServiceRecorder Create(ServiceRequest request);
    }
}
