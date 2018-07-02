using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Hosting;


using CodeArt.Web.WebPages;
using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    public class UploadFileInitializer : IAspect
    {
        private static UploadHttpModule _module = new UploadHttpModule();

        public void Before()
        {
            var context = WebPageContext.Current;
            var app = context.HttpContext.ApplicationInstance;
            try
            {
                _module.Process(app);
            }
            catch (Exception ex)
            {
                _module.End(app);
                throw ex;
            }
        }

        public void After()
        {
            var context = WebPageContext.Current;
            var app = context.HttpContext.ApplicationInstance;
            try
            {
                _module.End(app);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static UploadFileInitializer Instance = new UploadFileInitializer();

    }
}
