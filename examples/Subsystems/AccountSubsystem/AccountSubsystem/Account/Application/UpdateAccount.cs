using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class UpdateAccount : Command<Account>
    {
        public string Name
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

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

        public IList<Guid> RoleIds
        {
            get;
            set;
        }


        private Guid _id;

        public UpdateAccount(Guid id)
        {
            _id = id;
        }

        protected override Account ExecuteProcedure()
        {
            var repository = Repository.Create<IAccountRepository>();
            var account = repository.Find(_id, QueryLevel.Single);

            if (this.Name != null) account.Name = this.Name;
            if (this.Email != null) account.Email = this.Email;
            if (this.IsEnabled != null) account.Status.IsEnabled = this.IsEnabled.Value;
            if (this.MobileNumber != null) account.MobileNumber = this.MobileNumber;
            if (this.Password != null) account.SetPasswordAndEncrypt(this.Password);
            if (this.RoleIds != null)
            {
                var roles = RoleCommon.FindsBy(this.RoleIds);
                account.Roles = roles;
            }
            repository.Update(account);
            return account;
        }
    }
}
