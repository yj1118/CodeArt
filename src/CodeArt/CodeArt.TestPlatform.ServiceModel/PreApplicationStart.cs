using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.AppSetting;

[assembly: PreApplicationStart(typeof(CodeArt.TestPlatform.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.Low)]


namespace CodeArt.TestPlatform
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            Repository.Register<IServiceInvokeRepository>(SqlServiceInvokeRepository.Instance);
            CodeArt.ServiceModel.ServiceRecorderFactory.Register(ServiceRecorderFactory.Instance);
        }
    }
}