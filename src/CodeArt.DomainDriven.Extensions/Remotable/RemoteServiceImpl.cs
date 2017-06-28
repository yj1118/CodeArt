using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.Extensions
{
    [SafeAccess]
    internal class RemoteServiceImpl : IRemoteService
    {
        private RemoteServiceImpl() { }

        public DTObject GetObject(RemoteType remoteType, object id)
        {
            var serviceName = RemoteServiceName.GetObject(remoteType);
            return ServiceContext.Invoke(serviceName, (arg) =>
            {
                arg["id"] = id;
                arg["typeName"] = remoteType.FullName;
            });
        }


        public void NotifyUpdated(RemoteType remoteType, object id, IEnumerable<string> addresses)
        {
            var serviceName = RemoteServiceName.ObjectUpdated(remoteType);
            var arg = DTObject.CreateReusable();
            arg["id"] = id;
            arg["typeName"] = remoteType.FullName;

            Parallel.ForEach(addresses, (address) =>
            {
                AppSession.Using(() =>
                {
                    ServiceContext.Invoke(serviceName, arg, address);
                });
            });
        }

        public void NotifyDeleted(RemoteType remoteType, object id, IEnumerable<string> addresses)
        {
            var serviceName = RemoteServiceName.ObjectDeleted(remoteType);
            var arg = DTObject.CreateReusable();
            arg["id"] = id;
            arg["typeName"] = remoteType.FullName;

            Parallel.ForEach(addresses, (address) =>
            {
                ServiceContext.Invoke(serviceName, arg, address);
            });
        }

        public readonly static RemoteServiceImpl Instance = new RemoteServiceImpl();
    }
}
