
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;

using CodeArt.DTO;
using CodeArt.IO;
using CodeArt.Concurrent;
using CodeArt.Log;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    public sealed class WebServiceHost : IHttpHandler
    {
        public WebServiceHost()
        {

        }

        /// <summary>
        /// 可以重用
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            AppSession.Using(() =>
            {
                SetContentType(context);
                var requestType = context.Request.RequestType;

                if (requestType == "POST")
                    ProcessPOST(context);
                else
                    ProcessOther(context);
            }, true);
        }

        private void ProcessPOST(HttpContext context)
        {
            DTObject returnValue = null;
            DTObject status = null;
            try
            {
                var request = GetServiceRequest(context);
                InitContext(context, request);
                returnValue = ProcessService(request);
                status = ServiceHostUtil.Success;
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
                status = ServiceHostUtil.CreateFailed(ex);
            }
            finally
            {
                ServiceResponse response = new ServiceResponse(status, returnValue);
                SendResponse(context, response);
            }
        }

        private void SetContentType(HttpContext context)
        {
            context.Response.ContentType = "text/html";
        }

        private ServiceRequest GetServiceRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            var length = request.ContentLength;

            using (var poolItem = ByteBuffer.Borrow(length))
            {
                var data = poolItem.Item;
                data.Write(request.InputStream, length);
                return DataAnalyzer.DeserializeRequest(data);
            }
        }

        private DTObject ProcessService(ServiceRequest request)
        {
            var provider = ServiceProviderFactory.Create(request);
            return provider.Invoke(request);
        }

        private void SendResponse(HttpContext context, ServiceResponse response)
        {
            using (var poolItem = ByteBuffer.Borrow(Util.DefaultResponseSegmentSize))
            {
                var data = poolItem.Item;
                DataAnalyzer.SerializeResponse(response, data);
                data.Read(context.Response.OutputStream);
            }
        }

        private void InitContext(HttpContext context, ServiceRequest request)
        {
            WebServiceHost.Current = this;
            Identity = request.Identity;
        }

        public static WebServiceHost Current
        {
            get
            {
                return AppSession.GetItem<WebServiceHost>("WebServiceHost");
            }
            private set
            {
                AppSession.SetItem("WebServiceHost", value);
            }
        }

        public DTObject Identity
        {
            get
            {
                return AppSession.GetItem<DTObject>("__Identity");
            }
            private set
            {
                AppSession.SetItem("__Identity", value);
            }
        }

        private void ProcessOther(HttpContext context)
        {
            context.Response.Write("服务宿主正在运行中");
        }

        static WebServiceHost()
        {
            //当第一次使用ServiceHost时，初始化引用程序
            AppInitializer.Initialize();
        }


    }
}
