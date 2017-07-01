using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.ORM; 

namespace AccountSubsystem
{
    [SafeAccess]
    internal sealed class AccountSpecification : ObjectValidator<Account>
    {
        public AccountSpecification()
        {

        }

        protected override void Validate(Account obj, ValidationResult result)
        {
            if (!IsSafeName(obj))
            {
                result.AddError("Account.UnsafeName", string.Format(Strings.InvalidAccountName, obj.Name));
                return;
            }
            Validator.CheckPropertyRepeated(obj, Account.NameProperty, result);
            Validator.CheckPropertyRepeated(obj, Account.EmailProperty, result);
            Validator.CheckPropertyRepeated(obj, Account.MobileNumberProperty, result);
        }

        /// <summary>
        /// 账户的名称不能是邮箱或者手机号码
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool IsSafeName(Account account)
        {
            if (!account.IsPropertyDirty(Account.NameProperty)) return true; //属性不为脏，不需要验证
            return !IsEmail(account.Name) && !IsMobilenumber(account.Name);
        }

        private bool IsEmail(string name)
        {
            return EmailValidator.IsMatch(name);
        }

        private bool IsMobilenumber(string name)
        {
            return MobileNumber.CNValidator.IsMatch(name);
        }
    }
}
