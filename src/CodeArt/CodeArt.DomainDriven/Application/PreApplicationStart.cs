using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;


[assembly: PreApplicationStart(typeof(CodeArt.DomainDriven.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.Low)]


namespace CodeArt.DomainDriven
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            DomainObject.Initialize();
            LogableAttribute.Initialize();
        }
    }
}
