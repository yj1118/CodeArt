using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 提供请求方的身份，当客户端在调用服务A时，服务器A有可能会调用服务B，这时候可以使用该对象提供的身份，即：客户端使用的身份
    /// </summary>
    [SafeAccess]
    public class RequesterIdentityProvider : IIdentityProvider
    {
        /// <summary>
        /// 为当前服务上下文设置身份信息
        /// </summary>
        /// <param name="context"></param>
        public DTObject GetIdentity()
        {
            var identity = WebServiceHost.Current?.Identity;
            if (identity != null) return identity;
            throw new ServiceException(Web.Strings.WebServiceNotFoundIdentity);
        }
    }
}
