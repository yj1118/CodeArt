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
            AsyncExecuteService(serviceName, remoteType, id, addresses);
        }

        public void NotifyDeleted(RemoteType remoteType, object id, IEnumerable<string> addresses)
        {
            var serviceName = RemoteServiceName.ObjectDeleted(remoteType);
            AsyncExecuteService(serviceName, remoteType, id, addresses);
        }

        private void AsyncExecuteService(string serviceName, RemoteType remoteType, object id, IEnumerable<string> addresses)
        {
            AppSession.AsyncUsing(() =>
            {
                var arg = DTObject.CreateReusable();
                arg["id"] = id;
                arg["typeName"] = remoteType.FullName;

                var identity = DefaultIdentityProvider.Instance.GetIdentity();
                foreach (var address in addresses)
                {
                    ServiceContext.Invoke(serviceName, identity, arg, address);
                }
            }, true);
        }

        public readonly static RemoteServiceImpl Instance = new RemoteServiceImpl();
    }
}
