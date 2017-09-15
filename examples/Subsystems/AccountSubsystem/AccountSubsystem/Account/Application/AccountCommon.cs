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
            Account acc = AccountCommon.FindById(accountId, level);
            if (acc.IsEmpty()) throw new DomainDrivenException(string.Format("没有找到编号为 {0} 的帐户信息", accountId));
            return acc;
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
        /// <param name="password"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Account FindByFlag(string flag, string password, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindByFlag(flag, password, level);
        }

        public static IList<Account> FindsByRole(Guid roleId, QueryLevel level)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindsByRole(roleId, level);
        }

        public static Page<Account> FindPageBy(string name, string email, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IAccountRepository>();
            return repository.FindPageBy(name, email, pageIndex, pageSize);
        }
    }
}
