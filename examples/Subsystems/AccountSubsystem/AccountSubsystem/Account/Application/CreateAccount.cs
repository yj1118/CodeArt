using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class CreateAccount : Command<Account>
    {
        private string _name;
        private string _password;
        private Guid[] _roleIds;

        public string Email
        {
            get;
            set;
        }

        public string MobileNumber
        {
            get;
            set;
        }

        public bool? IsEnabled
        {
            get;
            set;
        }


        public CreateAccount(string name, string password, Guid[] roleIds)
        {
            _name = name;
            _password = password;
            _roleIds = roleIds;
        }

        protected override Account ExecuteProcedure()
        {
            var roles = _roleIds != null && _roleIds.Count() > 0 ? RoleCommon.FindsBy(_roleIds) : Array.Empty<Role>();

            Account acc = new Account(_name, _password, roles)
            {
                Email = this.Email ?? string.Empty,
                MobileNumber = this.MobileNumber ?? string.Empty
            };
            acc.Status.IsEnabled = this.IsEnabled ?? true;

            var repository = Repository.Create<IAccountRepository>();
            repository.Add(acc);

            return acc;
        }


    }
}
