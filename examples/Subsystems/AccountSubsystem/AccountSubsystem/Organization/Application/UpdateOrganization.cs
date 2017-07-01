using System;
using System.Collections.Generic;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace AccountSubsystem
{
    public sealed class UpdateOrganization : Command<Organization>
    {
        private Guid _id;

        public string Name
        {
            get;
            set;
        }

        public IList<Guid> PermissionIds
        {
            get;
            set;
        }

        public UpdateOrganization(Guid id)
        {
            _id = id;
        }

        protected override Organization ExecuteProcedure()
        {
            var repository = Repository.Create<IOrganizationRepository>();
            var org = repository.Find(_id, QueryLevel.Single);
            if (this.Name != null) org.Name = this.Name;
            if (this.PermissionIds != null)
            {
                var permissions = PermissionCommon.FindsBy(this.PermissionIds, QueryLevel.None);
                org.SetPermissions(permissions);
            }

            repository.Update(org);
            return org;
        }
    }
}
