using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

using SerialNumberSubsystem;
using SerialNumberSubsystem.DataAccess;

[assembly: PreApplicationStart(typeof(SerialNumberSubsystemTest.PreApplicationStart), "Initialize")]

namespace SerialNumberSubsystemTest
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<ISNGeneratorRepository>(SqlSNGeneratorRepository.Instance);
        }
    }
}
