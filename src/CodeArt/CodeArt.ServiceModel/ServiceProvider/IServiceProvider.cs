using System.ServiceModel;
using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public interface IServiceProvider
    {
        DTObject Invoke(ServiceRequest request);
    }
}