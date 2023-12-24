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
            var arg = DTObject.Create();
            fillArg(arg);
            return Invoke(serviceName, arg);
        }

        public static DTObject InvokeDynamic(string serviceName, Action<dynamic> fillArg)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            return Invoke(serviceName, arg);
        }


        public static DTObject Invoke(string serviceName, DTObject arg)
        {
            return Invoke(serviceName, AppContext.Identity, arg);
        }

        public static DTObject Invoke(string serviceName, DTObject identity, Action<DTObject> fillArg)
        {
            var arg = DTObject.Create();
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

        #region 二进制传输

        public static void InvokeBinaryDynamic(string serviceName, Action<dynamic> fillArg, Action<BinaryResponse> callBack)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            InvokeBinary(serviceName, arg, callBack);
        }

        public static void InvokeBinary(string serviceName, Action<DTObject> fillArg, Action<BinaryResponse> callBack)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            InvokeBinary(serviceName, arg, callBack);
        }


        public static void InvokeBinary(string serviceName, DTObject arg, Action<BinaryResponse> callBack)
        {
            InvokeBinary(serviceName, AppContext.Identity, arg, callBack);
        }

        public static void InvokeBinary(string serviceName, DTObject identity, DTObject arg, Action<BinaryResponse> callBack)
        {
            int length = 0;
            ServiceRequest request = new ServiceRequest(serviceName, identity, arg);

            request.TransmittedLength = length;
            var response = Send(request);
            var data = response.Binary;
            callBack(data);
            length += data.Content.Length;

            while ((data.Status & TransferStatus.Last) != TransferStatus.Last)
            {
                request.TransmittedLength = length;
                response = Send(request);
                data = response.Binary;
                callBack(data);
                length += data.Content.Length;
            }
        }

        #endregion

        #region 异步

        public static void AsyncInvoke(string serviceName, Action<DTObject> fillArg, Action<DTObject> success, Action<Exception> error)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            AsyncInvoke(serviceName, AppContext.Identity, arg, success, error);
        }

        public static void AsyncInvoke(string serviceName, Action<DTObject> fillArg, Action<Exception> error)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            AsyncInvoke(serviceName, AppContext.Identity, arg, null, error);
        }

        public static void AsyncInvoke(string serviceName, Action<DTObject> fillArg)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            AsyncInvoke(serviceName, AppContext.Identity, arg, null, null);
        }

        public static void AsyncInvoke(string serviceName, DTObject identity, DTObject arg,Action<DTObject> success, Action<Exception> error)
        {
            Task.Run(() =>
            {
                try
                {
                    ServiceRequest request = new ServiceRequest(serviceName, identity, arg);
                    var response = Send(request);
                    if(success != null)
                        success(response.ReturnValue);
                }
                catch (Exception ex)
                {
                    if (error != null)
                        error(ex);
                }
            });   
        }

        #endregion

        private static ServiceResponse Send(ServiceRequest request)
        {
            var response = ServiceProxy.Invoke(request);
            response.TryCatch();
            return response;
        }
    }
}
