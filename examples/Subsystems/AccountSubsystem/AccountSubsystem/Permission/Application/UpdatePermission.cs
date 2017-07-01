using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class UpdatePermission : Command<Permission>
    {
        private Guid _id;

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string MarkedCode
        {
            get;
            set;
        }

        public UpdatePermission(Guid id)
        {
            _id = id;
        }

        protected override Permission ExecuteProcedure()
        {
            IPermissionRepository repository = Repository.Create<IPermissionRepository>();
            var permission = repository.Find(_id, QueryLevel.Single);

            if (this.Name != null) permission.Name = this.Name;
            if (this.Description != null) permission.Description = this.Description;
            if (this.MarkedCode != null) permission.MarkedCode = this.MarkedCode;

            repository.Update(permission);
            return permission;
        }
    }
}
