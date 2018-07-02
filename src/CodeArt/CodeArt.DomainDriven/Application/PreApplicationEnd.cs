using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;

[assembly: PreApplicationEnd(typeof(CodeArt.DomainDriven.PreApplicationEnd), 
                                        "Cleanup", 
                                        PreApplicationStartPriority.Low)]

namespace CodeArt.DomainDriven
{
    internal class PreApplicationEnd
    {
        private static void Cleanup()
        {
            DomainObject.Cleanup();
        }
    }
}
