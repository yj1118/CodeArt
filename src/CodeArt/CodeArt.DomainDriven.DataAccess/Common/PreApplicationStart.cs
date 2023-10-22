using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

[assembly: PreApplicationStart(typeof(CodeArt.DomainDriven.DataAccess.PreApplicationStart), "Initialize", PreApplicationStartPriority.High)]

namespace CodeArt.DomainDriven.DataAccess
{
    public class PreApplicationStart
    {
        private static void Initialize()
        {
            //注入动态仓储的支持
            Repository.Register<IDynamicRepository>(SqlDynamicRepository.Instance);

            CodeArt.DomainDriven.LockManager.Register(LockManager.Instance);
        }

    }
}
