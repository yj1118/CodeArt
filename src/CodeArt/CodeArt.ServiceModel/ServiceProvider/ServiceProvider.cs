using System;
using System.ServiceModel;
using System.Collections.Generic;

using CodeArt.DTO;
using CodeArt.ServiceModel;

namespace CodeArt.ServiceModel
{
    public abstract class ServiceProvider : IServiceProvider
    {
        public virtual DTObject Invoke(ServiceRequest request)
        {
            return Invoke(request.Argument);
        }

        public virtual DTObject Invoke(DTObject arg)
        {
            return DynamicInvoke(arg);
        }

        protected virtual DTObject DynamicInvoke(dynamic arg)
        {
            return DTObject.Empty;
        }
    }
}
