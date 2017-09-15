using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using DomainDrivenTestApp.DomainModel;

[assembly: PreApplicationStart(typeof(DomainDrivenTestApp.PreApplicationStart),"Initialize")]


namespace DomainDrivenTestApp
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            Repository.Register<IUserRepository>(SqlUserRepository.Instance);
        }
    }
}
