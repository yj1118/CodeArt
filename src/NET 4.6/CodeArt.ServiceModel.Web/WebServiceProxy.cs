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


        /// <summary>
        /// 异步调用服务
        /// </summary>
        /// <param name="address"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> InvokeAsync(ServiceRequest request, string address)
        {
            ServiceResponse response;
            using (var requestDataItem = ByteBuffer.Borrow(SegmentSize.Byte256))
            {
                var requestData = requestDataItem.Item;
                DataAnalyzer.SerializeRequest(request, requestData);
                response = await HttpCommunicator.Instance.SendAsync(address, requestData);
            }
            return response;
        }

        /// <summary>
        /// 由于配置需要，构造函数是公开的
        /// </summary>
        public WebServiceProxy() { }

        public static readonly WebServiceProxy Instance = new WebServiceProxy();
    }
}
