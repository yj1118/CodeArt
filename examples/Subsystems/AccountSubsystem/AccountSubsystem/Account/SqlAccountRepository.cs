using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.DomainDriven.DataAccess;
using CodeArt.Concurrent;

namespace AccountSubsystem
{
    public interface IAccountRepository : IRepository<Account>
    {
        Account FindByName(string name, QueryLevel level);
        Account FindByEmail(string email, QueryLevel level);
        Account FindByMobileNumber(string mobileNumber, QueryLevel level);
        Account FindByFlag(string nameOrEmail, QueryLevel level);

        IEnumerable<Account> FindsByRole(Guid roleId, QueryLevel level);

        Page<Account> FindPageBy(string flag, int pageIndex, int pageSize);
    }

    [SafeAccess]
    public class SqlAccountRepository : SqlRepository<Account>, IAccountRepository
    {
        public static readonly SqlAccountRepository Instance = new SqlAccountRepository();

        public Account FindByName(string name, QueryLevel level)
        {
            return this.QuerySingle<Account>("name=@name", (arg) =>
            {
                arg.Add("name", name);
            }, level);
        }

        public Account FindByEmail(string email, QueryLevel level)
        {
            return this.QuerySingle<Account>("email=@email", (arg) =>
            {
                arg.Add("email", email);
            }, level);
        }

        public Account FindByFlag(string nameOrEmail, QueryLevel level)
        {
            return this.QuerySingle<Account>("email=@flag or name=@flag", (arg) =>
            {
                arg.Add("flag", nameOrEmail);
            }, level);
        }

        public Account FindByMobileNumber(string mobileNumber, QueryLevel level)
        {
            return this.QuerySingle<Account>("mobileNumber=@mobileNumber", (arg) =>
            {
                arg.Add("mobileNumber", mobileNumber);
            }, level);
        }

        public Page<Account> FindPageBy(string flag, int pageIndex, int pageSize)
        {
            return this.Query<Account>("@flag<name like @flag% or email like @flag%>[order by createTime]", pageIndex,pageSize, (arg) =>
            {
                arg.TryAdd("flag", flag);
            });
        }

        public IEnumerable<Account> FindsByRole(Guid roleId, QueryLevel level)
        {
            return this.Query<Account>("roles.id=@roleId",(arg) =>
            {
                arg.Add("roleId", roleId);
            }, level);
        }
    }
}
