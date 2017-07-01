using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

using AccountSubsystem;

[assembly:PreApplicationStart(typeof(PortalService.Application.PreApplicationStart), "Initialize")]

namespace PortalService.Application
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<IPermissionRepository>(SqlPermissionRepository.Instance);
            Repository.Register<IOrganizationRepository>(SqlOrganizationRepository.Instance);
            Repository.Register<IRoleRepository>(SqlRoleRepository.Instance);

            //orm配置
            SqlContext.RegisterAgent(SQLServerAgent.Instance);

#if DEBUG
            DataPortal.Dispose();
#endif
        }
    }
}
