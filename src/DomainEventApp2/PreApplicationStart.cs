using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;


[assembly: PreApplicationStart(typeof(DomainEventApp2.PreApplicationStart),"Initialize")]


namespace DomainEventApp2
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            EventHost.SetHosts("NodeEvent2");
        }
    }
}
