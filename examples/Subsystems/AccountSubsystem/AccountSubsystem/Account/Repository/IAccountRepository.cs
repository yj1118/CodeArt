using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CodeArt.DomainDriven;


namespace AccountSubsystem
{
    public interface IAccountRepository : IRepository<Account>
    {
        Account FindBy(Guid accountId, QueryLevel level);
        Account FindByName(string name, QueryLevel level);
        Account FindByEmail(string email, QueryLevel level);
        Account FindByMobileNumber(string mobileNumber, QueryLevel level);
        Account FindByFlag(string nameOrEmail, string password, QueryLevel level);

        IList<Account> FindsByRole(Guid roleId, QueryLevel level);

        IList<Account> FindPageBy(string name, string email, int pageSize, int pageIndex);
        int GetPageCount(string name, string email, QueryLevel level);
    }
}
