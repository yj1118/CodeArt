using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    /// <summary>
    /// 使用该命令可以删除任意类型的角色
    /// </summary>
    public class DeleteRolePro : Command
    {
        private Guid _id;

        public DeleteRolePro(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            RoleService.Delete(_id);
        }
    }
}
