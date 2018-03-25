using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;


[assembly: PreApplicationStart(typeof(DomainEventApp4.PreApplicationStart),"Initialize")]


namespace DomainEventApp4
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            EventHost.EnableEvent("NodeEvent4");
        }
    }
}
