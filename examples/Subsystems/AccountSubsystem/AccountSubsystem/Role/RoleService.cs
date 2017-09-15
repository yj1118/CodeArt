using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    internal static class RoleService
    {
        /// <summary>
        /// 删除角色，系统角色也可以被删除
        /// </summary>
        public static void DeleteSystem(Guid id)
        {
            Delete(id, true);
        }

        /// <summary>
        /// 删除普通角色
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteGeneral(Guid id)
        {
            Delete(id, false);
        }


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <param name="force">是否强制删除，true:可以删除系统角色，false:不能删除系统角色</param>
        private static void Delete(Guid id, bool force)
        {
            var repository = Repository.Create<IRoleRepository>();
            var role = repository.Find(id, QueryLevel.Single);

            if (!force && role.IsSystem)
            {
                throw new AccountException(string.Format(Strings.NotDeleteSystemRole, id));
            }

            repository.Delete(role);
        }
    }
}
