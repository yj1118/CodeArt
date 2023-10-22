using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.Log;
using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

namespace CodeArt.TestPlatform
{
    [SafeAccess]
    public class ServiceRecorderFactory : IServiceRecorderFactory
    {
        public IServiceRecorder Create(ServiceRequest request)
        {
            return ServiceRecorder.Instance;
        }

        public static readonly ServiceRecorderFactory Instance = new ServiceRecorderFactory();

    }
}
