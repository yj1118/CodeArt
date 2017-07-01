using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class CreatePermission : Command<Permission>
    {
        private string _name;

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

        public CreatePermission(string name)
        {
            _name = name;
        }

        protected override Permission ExecuteProcedure()
        {
            Permission permission = new Permission(Guid.NewGuid())
            {
                Name = _name,
                MarkedCode = this.MarkedCode ?? string.Empty,
                Description = this.Description ?? string.Empty
            };

            var repository = Repository.Create<IPermissionRepository>();
            repository.Add(permission);

            return permission;
        }
    }
}
