using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.ServiceModel;


[assembly: PreApplicationStart(typeof(CodeArt.ServiceModel.Web.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.High)]


namespace CodeArt.ServiceModel.Web
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            ServiceProxy.Register(WebServiceProxy.Instance);
        }
    }
}
