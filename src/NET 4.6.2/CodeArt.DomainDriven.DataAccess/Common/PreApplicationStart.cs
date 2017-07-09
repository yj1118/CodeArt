using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

[assembly: PreApplicationStart(typeof(CodeArt.DomainDriven.DataAccess.PreApplicationStart), "Initialize", PreApplicationStartPriority.Height)]

namespace CodeArt.DomainDriven.DataAccess
{
    public class PreApplicationStart
    {
        private static void Initialize()
        {
            Repository.Register<IDynamicRepository>(SqlDynamicRepository.Instance);

            CodeArt.DomainDriven.LockManager.Register(LockManager.Instance);
        }

    }
}
