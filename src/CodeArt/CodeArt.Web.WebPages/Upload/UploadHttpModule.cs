namespace CodeArt.Web.WebPages
{
    using System;
    using System.Reflection;
    using System.Web;
    using CodeArt.Web.WebPages;

    internal class UploadHttpModule
    {
        private void CloseConnectionAfterError(HttpResponse response)
        {
            response.GetType().InvokeMember("CloseConnectionAfterError", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, response, null);
        }

        public void Process(HttpApplication application)
        {
            HttpContext context = application.Context;
            if (Utility.GetEnableMemoryOptimizationIdentifier(context) == null && this.IsUploadRequest(application))
            {
                HttpWorkerRequest workerRequest = this.GetWorkerRequest(context);
                if (workerRequest != null)
                {
                    Type knownWorkerRequestType = this.GetKnownWorkerRequestType(workerRequest);
                    if (knownWorkerRequestType != null)
                    {
                        int requestLength = (int) RequestStream.GetRequestLength(workerRequest);
                        if (requestLength > this.GetMaxRequestLength(context))
                        {
                            this.ThrowMaximumRequestLengthExceeded(application.Response);
                        }
                        UploadContext uploadContext = this.CreateUploadContext(context, requestLength);
                        RequestParser parser = new RequestParser(this.GetBoundary(application.Request), application.Request.ContentEncoding, uploadContext);
                        parser.ProcessRequest(new RequestStream(workerRequest));
                        uploadContext.UploadedFiles = parser.UploadedFiles;
                        this.SetTextContent(knownWorkerRequestType, workerRequest, parser.TextContent);
                    }
                }
            }
        }

        public void End(HttpApplication application)
        {
            if (this.IsUploadRequest(application))
            {
                this.DisposeUploadContext(application.Context);
            }
        }

        private UploadContext CreateUploadContext(HttpContext context, int requestLength)
        {
            UploadContext uploadContext = new UploadContext(requestLength);
            UploadContext.SetUploadContext(context, uploadContext);
            return uploadContext;
        }

        public void Dispose()
        {
        }

        private void DisposeUploadContext(HttpContext context)
        {
            UploadContext uploadContext = UploadContext.GetUploadContext(context);
            if (uploadContext != null)
            {
                foreach (UploadedFile file in uploadContext.UploadedFiles)
                {
                    file.Delete();
                }
                UploadContext.RemoveUploadContext(context);
            }
        }

        private byte[] GetBoundary(HttpRequest request)
        {
            string contentType = request.ContentType;
            int index = contentType.IndexOf("boundary=");
            return ((index > 0) ? request.ContentEncoding.GetBytes("--" + contentType.Substring(index + "boundary=".Length)) : null);
        }

        private Type GetKnownWorkerRequestType(HttpWorkerRequest workerRequest)
        {
            return workerRequest.GetType();
            //Type baseType = workerRequest.GetType();
            //while ((((baseType != null) && (baseType.FullName != "System.Web.Hosting.ISAPIWorkerRequest")) && (baseType.FullName != "Cassini.Request")) && (baseType.FullName != "Microsoft.VisualStudio.WebHost.Request"))
            //{
            //    baseType = baseType.BaseType;
            //}
            //return baseType;
        }

        public int GetMaxRequestLength(HttpContext context)
        {
            object config = context.GetSection("system.web/httpRuntime");
            if (config == null)
            {
                return 0x400000;
            }
            PropertyInfo property = config.GetType().GetProperty("MaxRequestLength", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            int num = (property == null) ? 0x400000 : ((int) property.GetValue(config, null));
            return ((Environment.Version.Major > 1) ? (num *= 0x400) : num);
        }

        private string[] GetUploadIds(HttpApplication application)
        {
            string str = application.Request.QueryString[Utility.UPLOAD_IDS_QUERY_IDENTIFIER];
            switch (str)
            {
                case null:
                case "":
                    return new string[0];
            }
            return str.Split(new char[] { Utility.UPLOAD_IDS_QUERY_SEPARATOR });
        }

        private HttpWorkerRequest GetWorkerRequest(HttpContext context)
        {
            return (((IServiceProvider) context).GetService(typeof(HttpWorkerRequest)) as HttpWorkerRequest);
        }

        private bool IsUploadRequest(HttpApplication application)
        {
            return application.Request.ContentType.ToLower().StartsWith("multipart/form-data");
        }

        private void SetTextContent(Type workerRequestType, HttpWorkerRequest workerRequest, byte[] textContent)
        {
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var field = workerRequestType.GetField("_contentLength", bindingAttr);
            if(field != null) field.SetValue(workerRequest, textContent.Length);

            field = workerRequestType.GetField("_preloadedContent", bindingAttr);
            if (field != null) field.SetValue(workerRequest, textContent);

            field = workerRequestType.GetField("_preloadedContentLength", bindingAttr);
            if (field != null) field.SetValue(workerRequest, textContent.Length);

            field = workerRequestType.GetField("_contentAvailLength", bindingAttr);
            if (field != null) field.SetValue(workerRequest, textContent.Length);

            field = workerRequestType.GetField("_contentTotalLength", bindingAttr);
            if (field != null) field.SetValue(workerRequest, textContent.Length);

            field = workerRequestType.GetField("_preloadedContent", bindingAttr);
            if (field != null) field.SetValue(workerRequest, textContent);

            field = workerRequestType.GetField("_preloadedContentRead", bindingAttr);
            if (field != null) field.SetValue(workerRequest, true);
        }

        private void ThrowMaximumRequestLengthExceeded(HttpResponse response)
        {
            this.CloseConnectionAfterError(response);
            throw new HttpException(400, "Maximum request length exceeded.");
        }

        public static bool IsRegistered
        {
            get
            {
                HttpModuleCollection modules = HttpContext.Current.ApplicationInstance.Modules;
                foreach (string str in modules.AllKeys)
                {
                    if (modules[str] is UploadHttpModule)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

