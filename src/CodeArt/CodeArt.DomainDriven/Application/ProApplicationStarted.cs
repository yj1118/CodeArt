using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;


[assembly: ProApplicationStartedAttribute(typeof(CodeArt.DomainDriven.ProApplicationStarted),"Initialized")]


namespace CodeArt.DomainDriven
{
    internal class ProApplicationStarted
    {
        private static void Initialized()
        {
            DomainObject.Initialized();
        }
    }
}
