using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using AccountSubsystem;

namespace MenuSubsystem
{
    public static class MenuCommon
    {

        public static Menu FindBy(Guid menuId, QueryLevel level)
        {
            var repository = Repository.Create<IMenuRepository>();
            return repository.Find(menuId, level);
        }

        public static Menu FindBy(Guid menuId)
        {
            return FindBy(menuId, QueryLevel.None);
        }

        public static Menu FindBy(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<IMenuRepository>();
            return repository.FindBy(markedCode, level);
        }

        //public static IEnumerable<Menu> ShowMenu(Guid accountId, string markedCode, IMenuRender render)
        //{
        //    var account = AccountCommon.FindById(accountId);
        //    return ShowMenuService.Show(account, markedCode, render);
        //}

    }
}
