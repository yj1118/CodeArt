using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

using LocationSubsystem;

[assembly: PreApplicationStart(typeof(LocationSubsystemTest.PreApplicationStart), "Initialize")]

namespace LocationSubsystemTest
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<ILocationRepository>(SqlLocationRepository.Instance);
            SqlContext.RegisterAgent(SQLServerAgent.Instance);
        }
    }
}
