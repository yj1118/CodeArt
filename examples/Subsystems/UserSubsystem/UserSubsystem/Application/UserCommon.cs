using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using AccountSubsystem;
namespace UserSubsystem
{
    public static class UserCommon
    {
        //public static User FindByMobileNumber(string mobileNumber)
        //{
        //    var account = AccountCommon.FindByMobileNumber(mobileNumber, QueryLevel.None);
        //    return FindById(account.Id, QueryLevel.None);
        //}

        public static User FindById(Guid userId, QueryLevel level)
        {
            var repository = Repository.Create<IUserRepository>();
            return repository.Find(userId, level);
        }

        internal static IEnumerable<User> FindUsers(IEnumerable<Guid> userIds)
        {
            var repository = Repository.Create<IUserRepository>();
            return repository.FindUsers(userIds);
        }

        public static Page<User> FindPage(string name, string roleMarkedCode, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IUserRepository>();
            return repository.FindPage(name, roleMarkedCode, pageIndex, pageSize);
        }
    }
}
