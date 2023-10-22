using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using System.Timers;

[assembly: PreApplicationStart(typeof(CodeArt.DomainDriven.Extensions.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.Low)]

[assembly: PreApplicationStart(typeof(CodeArt.DomainDriven.Extensions.PreApplicationStart),
                                "Initialize_High",
                                PreApplicationStartPriority.High)]


namespace CodeArt.DomainDriven.Extensions
{
    internal class PreApplicationStart
    {
        private static void Initialize_High()
        {
            Repository.Register<IDomainMessageRepository>(SqlDomainMessageRepository.Instance);
            DomainMessageProvider.Reigster(DomainMessageWrapper.Instance);
        }


        private static void Initialize()
        {
            API.Initialize();
        }
    }
}
