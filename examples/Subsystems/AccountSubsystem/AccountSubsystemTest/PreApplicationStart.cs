using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

using AccountSubsystem;

[assembly: PreApplicationStart(typeof(AccountSubsystemTest.PreApplicationStart), "Initialize")]

namespace AccountSubsystemTest
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<IPermissionRepository>(SqlPermissionRepository.Instance);
            Repository.Register<IRoleRepository>(SqlRoleRepository.Instance);
        }
    }
}
