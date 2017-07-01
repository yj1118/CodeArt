using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public abstract class AccountRepositoryBase : Repository<Account>, IAccountRepository
    {
        #region Account FindBy(Guid accountId, QueryLevel level)

        protected abstract Account PersistFindBy(Guid accountId, QueryLevel level);

        public Account FindBy(Guid accountId, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistFindBy(accountId, level);
            this.RegisterQueried(ref result);
            return result;
        }

        #endregion

        #region Account FindByName(string name, QueryLevel level);

        protected abstract Account PersistFindByName(string name, QueryLevel level);

        public Account FindByName(string name, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistFindByName(name, level);
            this.RegisterQueried(ref result);
            return result;
        }

        #endregion

        #region Account FindByEmail(string email, QueryLevel level);

        protected abstract Account PersistFindByEmail(string email, QueryLevel level);

        public Account FindByEmail(string email, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistFindByEmail(email, level);
            this.RegisterQueried(ref result);
            return result;
        }

        #endregion

        #region Account FindByMobileNumber(string mobileNumber, QueryLevel level);

        protected abstract Account PersistFindByMobileNumber(string mobileNumber, QueryLevel level);

        public Account FindByMobileNumber(string mobileNumber, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistFindByMobileNumber(mobileNumber, level);
            this.RegisterQueried(ref result);
            return result;
        }

        #endregion

        #region Account FindByNameOrEmailAndPwd(string nameOrEmail ,string password, QueryLevel level);

        protected abstract Account PersistFindByFlagAndPwd(string nameOrEmail, string password, QueryLevel level);

        public Account FindByFlag(string nameOrEmail, string password, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistFindByFlagAndPwd(nameOrEmail, password, level);
            this.RegisterQueried(ref result);
            return result;
        }

        #endregion

        #region IList<Account> FindsByRole(Guid roleId, QueryLevel level);

        protected abstract IList<Account> PersistFindByRole(Guid roleId, QueryLevel level);

        public IList<Account> FindsByRole(Guid roleId, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistFindByRole(roleId, level);
            this.RegisterQueried(result);
            return result;
        }

        #endregion

        #region IList<Account> FindPageBy(string name, string email, int pageSize, int pageIndex);

        protected abstract IList<Account> PersistFindPageBy(string name, string email, int pageSize, int pageIndex);

        public IList<Account> FindPageBy(string name, string email, int pageSize, int pageIndex)
        {
            this.ManageLockLevel(QueryLevel.None);
            var result = PersistFindPageBy(name, email, pageSize, pageIndex);
            this.RegisterQueried(result);
            return result;
        }

        #endregion

        #region int GetPageCount(string name, string email, QueryLevel level);

        protected abstract int PersistGetPageCount(string name, string email, QueryLevel level);

        public int GetPageCount(string name, string email, QueryLevel level)
        {
            this.ManageLockLevel(level);
            var result = PersistGetPageCount(name, email, level);
            return result;
        }

        #endregion
    }
}
