using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

using CodeArt;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

using IServiceProvider = CodeArt.ServiceModel.IServiceProvider;

namespace CodeArt.DomainDriven.Extensions
{
    /// <summary>
    ///  该工厂类可以自动处理远程对象的事务而无需程序员操心
    ///  如果请求不是远程对象相关的服务，那么使用常规服务处理请求
    /// </summary>
    [SafeAccess]
    public class RemoteServiceProviderFactory : AttributeProviderFactory
    {

        public override IServiceProvider Create(ServiceRequest request)
        {
            return GetProviderByRemote(request) ?? base.Create(request);
        }

        private IServiceProvider GetProviderByRemote(ServiceRequest request)
        {
            var fullName = request.FullName;
            if (fullName.StartsWith("d:") || fullName.StartsWith("d."))
            {
                return _getProviderByRemote(fullName);
            }
            return null;
        }

        private static Func<string, IServiceProvider> _getProviderByRemote = LazyIndexer.Init<string, IServiceProvider>((serviceFullName) =>
        {
            //先尝试获取用户自定义的服务
            var service = ServiceAttribute.GetServiceImpl(serviceFullName);
            if (service != null) return service;

            var temp = serviceFullName.Split(':');
            var ns = temp[0];
            var name = temp[1];
            if (name.StartsWith("Get")) return GetRemoteObject.Instance;
            if (name.EndsWith("Updated")) return RemoteObjectUpdated.Instance;
            if (name.EndsWith("Deleted")) return RemoteObjectDeleted.Instance;
            throw new DomainDrivenException(string.Format(Strings.GetServiceUnexpectedError, serviceFullName));
        });



        public new static readonly RemoteServiceProviderFactory Instance = new RemoteServiceProviderFactory();
    }
}
