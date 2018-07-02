using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Concurrent;
using CodeArt.EasyMQ.Event;

using SerialNumberSubsystem;
using SerialNumberSubsystem.DataAccess;

[assembly:PreApplicationStart(typeof(SerialNumberService.Application.PreApplicationStart), "Initialize")]

namespace SerialNumberService.Application
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<ISNGeneratorRepository>(SqlSNGeneratorRepository.Instance);

            //orm配置，默认就是使用的SQLServerAgent
            //SqlContext.RegisterAgent(SQLServerAgent.Instance);

#if DEBUG
            DataPortal.Dispose();
#endif
        }
    }
}
