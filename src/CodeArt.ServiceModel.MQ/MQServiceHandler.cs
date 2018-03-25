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
using System.Threading;
using System.Globalization;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    public class MQServiceHandler : IRPCHandler
    {
        protected MQServiceHandler() { }

        public DTObject Process(string method, DTObject arg)
        {
            DTObject returnValue = null;
            DTObject status = null;
            try
            {
                var request = ServiceRequest.Create(arg);
                InitIdentity(request);
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

        private void InitIdentity(ServiceRequest request)
        {
            AppSession.Identity = request.Identity;
        }


        protected virtual DTObject ProcessService(ServiceRequest request)
        {
            var provider = ServiceProviderFactory.Create(request);
            return provider.Invoke(request);
        }

        public static readonly MQServiceHandler Instance = new MQServiceHandler();

    }
}
