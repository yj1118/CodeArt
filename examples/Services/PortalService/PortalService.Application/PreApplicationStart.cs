using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt;
using CodeArt.ServiceModel;

using AccountSubsystem;
using LocationSubsystem;
using UserSubsystem;
using FileSubsystem;

[assembly:PreApplicationStart(typeof(PortalService.Application.PreApplicationStart), "Initialize")]

namespace PortalService.Application
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            //语言选项使用基于ServiceHost的语言提供器
            LanguageProvider.Register(ServiceHostLanguageProvider.Instance);

            Repository.Register<IPermissionRepository>(SqlPermissionRepository.Instance);
            Repository.Register<IRoleRepository>(SqlRoleRepository.Instance);
            Repository.Register<IAccountRepository>(SqlAccountRepository.Instance);
            Repository.Register<ILocationRepository>(SqlLocationRepository.Instance);
            Repository.Register<IUserRepository>(SqlUserRepository.Instance);

            Repository.Register<IVirtualDiskRepository>(SqlVirtualDiskRepository.Instance);
            Repository.Register<IVirtualDirectoryRepository>(SqlVirtualDirectoryRepository.Instance);
            Repository.Register<IVirtualFileRepository>(SqlVirtualFileRepository.Instance);

            //orm配置
            SqlContext.RegisterAgent(SQLServerAgent.Instance);
            //基于配置文件的连接
            SqlContext.RegisterConnectionProvider(CodeArt.Data.SqlConnectionProvider.Instance);
//#if DEBUG
//            DataPortal.Dispose();
//#endif
        }
    }
}
