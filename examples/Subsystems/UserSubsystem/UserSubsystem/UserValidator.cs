using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace UserSubsystem
{
    [SafeAccess]
    internal sealed class UserValidator : ObjectValidator<User>
    {

        protected override void Validate(User obj, ValidationResult result)
        {
            if (ExistsSameAccount(obj))
                result.AddError("User.AccountRepeated", string.Format(Strings.RepeatedAssignAccount, obj.Account.Name));
        }

        private bool ExistsSameAccount(User obj)
        {
            User target = UserCommon.FindById(obj.Account.Id, QueryLevel.HoldSingle);
            if (target.IsEmpty()) return false;
            if (target.Equals(obj)) return false;
            return true;
        }
    }
}