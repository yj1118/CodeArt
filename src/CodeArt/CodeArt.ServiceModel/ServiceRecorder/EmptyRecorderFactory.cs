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
    internal class EmptyRecorderFactory : IServiceRecorderFactory
    {
        public IServiceRecorder Create(ServiceRequest request)
        {
            return EmptyRecorder.Instance;
        }

        public static readonly EmptyRecorderFactory Instance = new EmptyRecorderFactory();

    }
}
