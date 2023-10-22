using CodeArt.AppSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Security;

[assembly: ProApplicationStarted(typeof(CodeArt.ServiceModel.ProApplicationStarted), "Initialized")]

namespace CodeArt.ServiceModel
{
    internal class ProApplicationStarted
    {
        public static void Initialized()
        {
            //ServiceProvider.Subscribe();
        }
    }
}
