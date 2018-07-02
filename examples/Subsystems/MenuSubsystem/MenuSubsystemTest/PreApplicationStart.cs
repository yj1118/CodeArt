using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

using MenuSubsystem;
using TreeSubsystem;

[assembly: PreApplicationStart(typeof(MenuSubsystemTest.PreApplicationStart), "Initialize")]

namespace MenuSubsystemTest
{
    internal class PreApplicationStart
    {
        public static void Initialize()
        {
            Repository.Register<ITreeNodeRepository>(SqlTreeNodeRepository.Instance);
            Repository.Register<IMenuRepository>(SqlMenuRepository.Instance);

            //orm配置
            //SqlContext.RegisterAgent(SQLServerAgent.Instance);
            //SqlContext.RegisterMapper<Menu>(MenuMapper.Instance);
        }
    }
}
