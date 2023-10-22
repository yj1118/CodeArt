using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    internal class LockManager : ILockManager
    {
        public void Lock(IEnumerable<IAggregateRoot> roots)
        {
            roots = GetSecuritySequence(roots); //获得安全序列
            using (var temp = ListPool<Type>.Borrow())
            {
                var types = temp.Item;
                using (var temp2 = ListPool<object>.Borrow())
                {
                    var ids = temp2.Item;
                    foreach (var root in roots)
                    {
                        types.Add(((DomainObject)root).ObjectType);
                        ids.Add(root.GetIdentity());
                    }
                    ExecuteLockSql(types, ids);
                }
            }
        }

        /// <summary>
        /// 令所有的锁定操作都遵循一种排序的方式进行，这样为安全序列
        /// 在安全序列里是不会造成死锁的
        /// </summary>
        /// <param name="roots"></param>
        /// <returns></returns>
        private IEnumerable<IAggregateRoot> GetSecuritySequence(IEnumerable<IAggregateRoot> roots)
        {
            return roots.OrderBy((root) =>
            {
                return ((DomainObject)root).ObjectType.Name;
            }).OrderBy((root) =>
            {
                return root.GetIdentity().ToString();
            });
        }

        /// <summary>
        ///  以安全序列的方式执行锁定的sql语句
        /// </summary>
        /// <param name="types"></param>
        /// <param name="ids"></param>
        private void ExecuteLockSql(List<Type> types, List<object> ids)
        {
            for (var i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var id = ids[i];

                bool multiTenancy = DomainDrivenConfiguration.Current.MultiTenancyConfig.IsEnabled;

                using (var temp = SqlHelper.BorrowData())
                {
                    var arg = temp.Item;
                    arg.Add(EntityObject.IdPropertyName, id);
                    if (multiTenancy)
                        arg.Add(GeneratedField.TenantIdName, AppSession.TenantId);

                    var table = DataModel.Create(type).Root;
                    var sql = SingleLock.Create(table).Build(null, table);
                    //DataContext.Current.Connection.Execute(sql, arg);
                    SqlHelper.Execute(sql, arg);
                }
            }
        }

        public static readonly LockManager Instance = new LockManager();
    }
}
