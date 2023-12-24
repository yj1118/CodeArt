using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

[assembly: PreApplicationEnd(typeof(CodeArt.ServiceModel.PreApplicationEnd), 
                                        "Cleanup", 
                                        PreApplicationStartPriority.Low)]

namespace CodeArt.ServiceModel
{
    internal class PreApplicationEnd
    {
        private static void Cleanup()
        {
            //ServiceProvider.Cancel();
        }
    }
}
