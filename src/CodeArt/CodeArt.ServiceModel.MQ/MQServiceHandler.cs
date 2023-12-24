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
using CodeArt.EasyMQ;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    public class MQServiceHandler : IRPCHandler
    {
        protected MQServiceHandler() { }

        public TransferData Process(string method, DTObject arg)
        {
            if (!MQServiceHost.IsEnabled) throw new UserUIException("正在启动服务，请稍候");

            DTObject returnValue = null;
            DTObject status = null;
            int dataLength = 0;
            byte[] content = null;

            try
            {
                var request = ServiceRequest.Create(arg);
                InitIdentity(request);

                if (request.TransmittedLength == null)
                {
                    returnValue = ProcessService(request);
                }
                else
                {
                    var result = ProcessDownloadService(request);
                    returnValue = result.Info;
                    dataLength = result.DataLength;
                    content = result.Content;
                }
                
                status = ServiceHostUtil.Success;
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                status = ServiceHostUtil.CreateFailed(ex);
            }

            var reponse = DTObject.Create();
            reponse["status"] = status;
            reponse["returnValue"] = returnValue;
            return new TransferData(AppSession.Language, reponse, dataLength, content);
        }

        private void InitIdentity(ServiceRequest request)
        {
            AppSession.Identity = request.Identity;
        }


        private DTObject ProcessService(ServiceRequest request)
        {
            var provider = ServiceProviderFactory.Create(request);
            return provider.Invoke(request);
        }

        private BinaryData ProcessDownloadService(ServiceRequest request)
        {
            var provider = ServiceProviderFactory.Create(request);
            return provider.InvokeBinary(request);
        }

        public static readonly MQServiceHandler Instance = new MQServiceHandler();

    }
}
