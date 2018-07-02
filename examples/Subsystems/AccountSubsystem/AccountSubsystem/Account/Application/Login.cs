using System;
using System.Linq;
using System.Collections.Generic;

using CodeArt;
using CodeArt.DomainDriven;
using AccountSubsystem;

namespace AccountSubsystem
{
    public sealed class Login : Command<Account>
    {
        private string _flag;
        private string _password;
        private string _ip;

        public Login(string flag, string password, string ip)
        {
            _flag = flag;
            _password = password;
            _ip = ip;
        }

        protected override Account ExecuteProcedure()
        {
            var repository = Repository.Create<IAccountRepository>();
            Account ac = repository.FindByFlag(_flag, QueryLevel.Mirroring);
            if (ac.IsEmpty()) throw new LoginFailException(Strings.AccountNameOrPasswordWrong);
            if(!ac.ValidatePassword(_password)) throw new LoginFailException(Strings.AccountNameOrPasswordWrong);
            if (!ac.Status.IsEnabled) throw new LoginFailException(Strings.AccountDisabled);
            ac.Status.UpdateLogin(_ip);
            repository.Update(ac);
            return ac;
        }
    }
}
