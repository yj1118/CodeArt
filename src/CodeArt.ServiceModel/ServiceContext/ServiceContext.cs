using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;

using CodeArt;
using CodeArt.DTO;
using System.Text;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    public static class ServiceContext
    {
        #region 同步

        public static DTObject Invoke(string serviceName)
        {
            return Invoke(serviceName, DTObject.Empty);
        }

        public static DTObject Invoke(string serviceName, Action<DTObject> fillArg)
        {
            var arg = DTObject.CreateReusable();
            fillArg(arg);
            return Invoke(serviceName, arg);
        }

        public static DTObject Invoke(string serviceName, Action<dynamic> fillArg)
        {
            var arg = DTObject.CreateReusable();
            fillArg(arg);
            return Invoke(serviceName, arg);
        }


        public static DTObject Invoke(string serviceName, DTObject arg)
        {
            return Invoke(serviceName, _defaultIdentity, arg);
        }

        public static DTObject Invoke(string serviceName, DTObject identity, Action<DTObject> fillArg)
        {
            var arg = DTObject.CreateReusable();
            fillArg(arg);
            return Invoke(serviceName, identity, arg);
        }

        /// <summary>
        /// 指定身份发送请求
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="identity"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static DTObject Invoke(string serviceName, DTObject identity, DTObject arg)
        {
            ServiceRequest request = new ServiceRequest(serviceName, identity, arg);
            var response = Send(request);
            return response.ReturnValue;
        }

        #endregion

        private static ServiceResponse Send(ServiceRequest request)
        {
            var response = ServiceProxy.Invoke(request);
            response.TryCatch();
            return response;
        }

        private static DTObject _defaultIdentity = DTObject.Create();

        static ServiceContext()
        {
            _defaultIdentity.SetValue("name", Configuration.Current.Authentication.Identity.Name);
            _defaultIdentity.SetValue("tokenKey", string.Empty);
        }


    }
}
