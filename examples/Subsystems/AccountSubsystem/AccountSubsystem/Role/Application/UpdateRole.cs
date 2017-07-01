using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class UpdateRole : Command<Role>
    {
        public string Name
        {
            get;
            set;
        }

        public string MarkedCode
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public IList<Guid> PermissionIds
        {
            get;
            set;
        }

        private Guid _id;

        public UpdateRole(Guid id)
        {
            _id = id;
        }

        protected override Role ExecuteProcedure()
        {
            var repository = Repository.Create<IRoleRepository>();
            Role role = repository.Find(_id, QueryLevel.Single);

            if (this.Name != null) role.Name = this.Name;
            if (this.MarkedCode != null) role.MarkedCode = this.MarkedCode;
            if (this.Description != null) role.Description = this.Description;
            if (this.PermissionIds != null)
            {
                var permissions = PermissionCommon.FindsBy(this.PermissionIds, QueryLevel.None);
                role.SetPermissions(permissions);
            }
            repository.Update(role);
            return role;
        }
    }
}
