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
        private string _twicePassword;
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

        public CreateAccount(string name, string password, string twicePassword, Guid[] roleIds)
        {
            _name = name;
            _password = password;
            _twicePassword = twicePassword;
            _roleIds = roleIds;
        }

        protected override Account ExecuteProcedure()
        {
            if (!ConfirmPassword())
                throw new DomainDrivenException("两次密码不同！");

            var roles = RoleCommon.FindsBy(_roleIds);

            Account acc = new Account(_name, _password, roles)
            {
                Email = this.Email ?? string.Empty,
                MobileNumber = this.MobileNumber ?? string.Empty
            };

            var repository = Repository.Create<IAccountRepository>();
            repository.Add(acc);

            return acc;
        }

        private bool ConfirmPassword()
        {
            return _password.Equals(_twicePassword, StringComparison.Ordinal);
        }


    }
}
