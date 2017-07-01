using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    /// <summary>
    /// 创建一个组织
    /// </summary>
    public class CreateOrganization : Command<Organization>
    {
        private Guid _id;

        public string Name
        {
            get;
            set;
        }

        public CreateOrganization(Guid id)
        {
            _id = id;
            this.Name = string.Empty;
        }

        protected override Organization ExecuteProcedure()
        {
            var org = new Organization(_id);
            org.Name = this.Name;

            var repository = Repository.Create<IOrganizationRepository>();
            repository.Add(org);
            return org;
        }
    }
}
