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
using CodeArt.EasyMQ.RPC;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    internal sealed class MQServiceProxy : IServiceProxy
    {
        public ServiceResponse Invoke(ServiceRequest request)
        {
            DTObject args = DTObject.CreateReusable();
            args["serviceName"] = request.FullName;
            args["identity"] = request.Identity;
            args["argument"] = request.Argument;

            var result = RPCClient.Invoke(request.FullName, args);
            return ServiceResponse.Create(result);
        }

        /// <summary>
        /// 由于配置需要，构造函数是公开的
        /// </summary>
        public MQServiceProxy() { }

        public static readonly MQServiceProxy Instance = new MQServiceProxy();
    }
}
