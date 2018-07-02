using System;
using System.Collections.Generic;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.DomainDriven.DataAccess.SQLServer;

namespace UserSubsystem
{
    public partial interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Page<User> FindPage(string name, string roleMarkedCode, int pageIndex, int pageSize);

        IEnumerable<User> FindUsers(IEnumerable<Guid> userIds);
    }

    [SafeAccess]
    public class SqlUserRepository : SqlRepository<User>, IUserRepository
    {
        private SqlUserRepository() { }

        public static readonly IUserRepository Instance = new SqlUserRepository();

        public Page<User> FindPage(string name, string roleMarkedCode, int pageIndex, int pageSize)
        {
            //以下语句查询报错，待解决todo
            //return this.Query<User>("@name<name like @name%> and @roleMarkedCode<account.roles.markedCode=@roleMarkedCode>[order by account.createTime]", pageIndex, pageSize, (data) =>
            //{
            //    data.TryAdd("name", name);
            //    data.TryAdd("roleMarkedCode", roleMarkedCode);
            //});
            return this.Query<User>("@name<name like @name%>[order by account.createTime]", pageIndex, pageSize, (data) =>
            {
                data.TryAdd("name", name);
                data.TryAdd("roleMarkedCode", roleMarkedCode);
            });
        }

        public IEnumerable<User> FindUsers(IEnumerable<Guid> userIds)
        {
            return this.Query<User>("id in @ids",(data) =>
            {
                data.Add("ids", userIds);
            },QueryLevel.None);
        }
    }
}
