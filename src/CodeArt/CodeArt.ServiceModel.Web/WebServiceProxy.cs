using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;

using CodeArt;
using CodeArt.DTO;
using CodeArt.IO;
using CodeArt.Concurrent;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    public sealed class WebServiceProxy : IServiceProxy
    {
        public ServiceResponse Invoke(ServiceRequest request)
        {
            var address = GetAddress(request);
            return Invoke(request, address);
        }

        public ServiceResponse Invoke(ServiceRequest request,string address)
        {
            ServiceResponse response;
            using (var requestDataItem = ByteBuffer.Borrow(Util.DefaultRequestSegmentSize))
            {
                var requestData = requestDataItem.Item;
                DataAnalyzer.SerializeRequest(request, requestData);
                response = HttpCommunicator.Instance.Send(address, requestData);
            }
            return response;
        }


        private static string GetAddress(ServiceRequest request)
        {
            var address = ServiceRouter.Instance.GetAddress(request);
            if (string.IsNullOrEmpty(address))
                throw new ServiceException(string.Format(Web.Strings.NotFoundServiceAddress, request.Namespace, request.Name));
            return address;
        }

        /// <summary>
        /// 由于配置需要，构造函数是公开的
        /// </summary>
        public WebServiceProxy() { }

        public static readonly WebServiceProxy Instance = new WebServiceProxy();
    }
}
