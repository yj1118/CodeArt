using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public interface IIdentityProvider
    {
        /// <summary>
        /// 获取请求服务的身份信息
        /// </summary>
        DTObject GetIdentity();
    }
}
