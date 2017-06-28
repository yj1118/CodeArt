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
            var identity = GetIdentity();
            return Invoke(serviceName, identity, arg);
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

        public static DTObject Invoke(string serviceName, DTObject arg, string address)
        {
            var identity = GetIdentity();
            ServiceRequest request = new ServiceRequest(serviceName, identity, arg);
            var response = Send(request, address);
            return response.ReturnValue;
        }

        #endregion

        #region 异步

        public static void InvokeAsync(string serviceName, Action<DTObject> success, Action<Exception> error = null)
        {
            InvokeAsync(serviceName, DTObject.Empty, success, error);
        }

        public static void InvokeAsync(string serviceName, Action<DTObject> fillArg,
                                        Action<DTObject> success, Action<Exception> error = null)
        {
            var arg = DTObject.CreateReusable();
            fillArg(arg);
            InvokeAsync(serviceName, arg, success, error);
        }

        public static async void InvokeAsync(string serviceName, DTObject identity, DTObject arg,
                                        Action<DTObject> success, Action<Exception> error = null)
        {
            ServiceRequest request = new ServiceRequest(serviceName, identity, arg);
            try
            {
                var response = await SendAsync(request);
                var result = response.ReturnValue;
                if (success != null) success(result);
            }
            catch (Exception ex)
            {
                if (error != null) error(ex);
            }
        }

        public static void InvokeAsync(string serviceName, DTObject arg,
                                        Action<DTObject> success, Action<Exception> error = null)
        {
            var identity = GetIdentity();
            InvokeAsync(serviceName, identity, arg, success, error);
        }

        #endregion


        private static DTObject GetIdentity()
        {
            var identityProvider = ServiceModelConfiguration.Current.Client.GetIdentityProvider();
            return identityProvider.GetIdentity();
        }

        private static ServiceResponse Send(ServiceRequest request)
        {
            var address = GetAddress(request);
            return Send(request, address);
        }

        private static ServiceResponse Send(ServiceRequest request, string address)
        {
            var response = ServiceProxy.Invoke(request, address);
            response.TryCatch();
            return response;
        }

        private static async Task<ServiceResponse> SendAsync(ServiceRequest request)
        {
            var address = GetAddress(request);
            var response = await ServiceProxy.InvokeAsync(request, address);
            response.TryCatch();
            return response;
        }

        private static string GetAddress(ServiceRequest request)
        {
            var address = ServiceRouter.Instance.GetAddress(request);
            if (string.IsNullOrEmpty(address))
                throw new ServiceException(string.Format(Strings.NotFoundServiceAddress, request.Namespace, request.Name));
            return address;
        }

    }
}
