using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

[assembly: PreApplicationStart(typeof(DomainEventApp0.PreApplicationStart),"Initialize")]


namespace DomainEventApp0
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            DataPortal.Dispose();

            EventHost.EnableEvent("NodeEvent0");
        }
    }
}
