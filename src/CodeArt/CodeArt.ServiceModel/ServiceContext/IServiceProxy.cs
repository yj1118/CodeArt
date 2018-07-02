using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

using CodeArt;

namespace CodeArt.ServiceModel
{
    public interface IServiceProxy
    {
        /// <summary>
        /// 调用服务
        /// </summary>
        /// <param name="address"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        ServiceResponse Invoke(ServiceRequest request);
    }
}
