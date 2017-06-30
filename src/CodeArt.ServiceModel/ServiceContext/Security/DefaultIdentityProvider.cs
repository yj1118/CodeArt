using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    public class DefaultIdentityProvider : IIdentityProvider
    {
        private static DTObject _identity = DTObject.Create();

        static DefaultIdentityProvider()
        {
            _identity.SetValue("name", Configuration.Current.Authentication.Identity.Name);
            _identity.SetValue("tokenKey", string.Empty);
        }

        private DefaultIdentityProvider() { }

        public static readonly DefaultIdentityProvider Instance = new DefaultIdentityProvider();

        /// <summary>
        /// 为当前服务上下文设置身份信息
        /// </summary>
        /// <param name="context"></param>
        public DTObject GetIdentity()
        {
            return _identity;
        }
    }
}
