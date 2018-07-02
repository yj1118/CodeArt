using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public static class AccountCommon
    {
        public static Account FindById(Guid accountId, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.Find(accountId, level);
        }

        internal static Account FindByName(string name, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindByName(name, level);
        }

        internal static Account FindByEmail(string name, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindByEmail(name, level);
        }

        public static Account FindByMobileNumber(string mobileNumber, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindByMobileNumber(mobileNumber, level);
        }

        /// <summary>
        /// 根据账号名或邮箱或手机号查找账户
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Account FindByFlag(string flag, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindByFlag(flag, level);
        }

        public static IEnumerable<Account> FindsByRole(Guid roleId, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindsByRole(roleId, level);
        }

        public static Page<Account> FindPage(string flag, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindPageBy(flag, pageIndex, pageSize);
        }
    }
}
