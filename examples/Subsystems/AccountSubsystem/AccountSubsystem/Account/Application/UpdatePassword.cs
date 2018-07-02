using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public class UpdatePassword : Command<Account>
    {
        private Guid _accountId;

        private string _accountName;

        private string _oldPassword;

        private string _newPassword;

        public UpdatePassword(Guid accountId, string accountName, string oldPassword, string newPassword)
        {
            _accountId = accountId;
            _accountName = accountName;
            _oldPassword = oldPassword;
            _newPassword = newPassword;
        }

        protected override Account ExecuteProcedure()
        {
            var repository = Repository.Create<IAccountRepository>();
            var account = repository.Find(_accountId, QueryLevel.Single);

            if (account.Name == _accountName && account.ValidatePassword(_oldPassword))
            {
                account.SetPasswordAndEncrypt(_newPassword);
                repository.Update(account);
            }
            else
                throw new UpdatePasswordFailException(Strings.UpdatePasswordError);
            return account;
        }
    }
}
