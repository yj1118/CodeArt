using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

using CodeArt;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 基于特性的服务提供工厂
    /// </summary>
    [SafeAccess]
    public class AttributeProviderFactory : IServiceProviderFactory
    {
        public virtual IServiceProvider Create(ServiceRequest request)
        {
            return ServiceAttribute.GetService(request.FullName);
        }

        public static readonly AttributeProviderFactory Instance = new AttributeProviderFactory();

    }
}
