using System.ServiceModel;
using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public interface IServiceProvider
    {
        DTObject Invoke(ServiceRequest request);

        /// <summary>
        /// 提供二进制传输服务（totalBinaryData是数据的总长度，不代表本次传输的数据长度）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        BinaryData InvokeBinary(ServiceRequest request);
    }
}