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
            DTObject args = DTObject.Create();
            args["serviceName"] = request.FullName;
            args["identity"] = request.Identity;
            args["argument"] = request.Argument;
            if (request.TransmittedLength != null)
            {
                return InvokeBinary(request, args);
            }
            else
            {
                return InvokeDTO(request, args);
            }
        }

        private ServiceResponse InvokeBinary(ServiceRequest request, DTObject args)
        {
            int transmittedLength = request.TransmittedLength.Value;
            args["transmittedLength"] = transmittedLength;
            var result = RPCClient.Invoke(request.FullName, args);

            var status = TransferStatus.Ing;

            if (transmittedLength == 0)
            {
                status |= TransferStatus.First;
            }

            var bufferLength = result.Buffer.Length;
            if (result.DataLength == (transmittedLength + bufferLength))
            {
                status |= TransferStatus.Last;
            }

            return ServiceResponse.Create(result.Info, new BinaryResponse(status, result.Buffer));
        }

        private ServiceResponse InvokeDTO(ServiceRequest request, DTObject args)
        {
            var result = RPCClient.Invoke(request.FullName, args);
            return ServiceResponse.Create(result.Info, BinaryResponse.Empty);
        }

        /// <summary>
        /// 由于配置需要，构造函数是公开的
        /// </summary>
        public MQServiceProxy() { }

        public static readonly MQServiceProxy Instance = new MQServiceProxy();
    }
}
