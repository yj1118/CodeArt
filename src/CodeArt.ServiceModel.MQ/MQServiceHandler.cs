using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;

using CodeArt;
using CodeArt.DTO;
using CodeArt.Log;
using CodeArt.Concurrent;
using CodeArt.EasyMQ.RPC;
using CodeArt.AppSetting;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    internal class MQServiceHandler : IRPCHandler
    {
        private MQServiceHandler() { }

        public DTObject Process(string method, DTObject args)
        {
            DTObject returnValue = null;
            DTObject status = null;
            try
            {
                var request = ServiceRequest.Create(args);
                InitHost(request);
                returnValue = ProcessService(request);
                status = ServiceHostUtil.Success;
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
                status = ServiceHostUtil.CreateFailed(ex);
            }

            var reponse = DTObject.CreateReusable();
            reponse["status"] = status;
            reponse["returnValue"] = returnValue;
            return reponse;
        }

        private void InitHost(ServiceRequest request)
        {
            ServiceHost.Identity = request.Identity;
        }


        private DTObject ProcessService(ServiceRequest request)
        {
            var provider = ServiceProviderFactory.Create(request);
            return provider.Invoke(request);
        }

        public static readonly MQServiceHandler Instance = new MQServiceHandler();

    }
}
