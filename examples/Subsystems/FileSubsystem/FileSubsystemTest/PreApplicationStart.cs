using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

using FileSubsystem;

[assembly: PreApplicationStart(typeof(FileSubsystemTest.PreApplicationStart), "Initialize")]

namespace FileSubsystemTest
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<IVirtualDiskRepository>(SqlVirtualDiskRepository.Instance);
            Repository.Register<IVirtualDirectoryRepository>(SqlVirtualDirectoryRepository.Instance);
            Repository.Register<IVirtualFileRepository>(SqlVirtualFileRepository.Instance);

            SqlContext.RegisterAgent(SQLServerAgent.Instance);
        }
    }
}
