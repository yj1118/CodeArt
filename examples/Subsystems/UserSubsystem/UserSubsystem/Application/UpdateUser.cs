using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DomainDriven;
using LocationSubsystem;
using FileSubsystem;
using AccountSubsystem;

namespace UserSubsystem
{
    public class UpdateUser : Command<User>
    {
        private Guid _id;

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex? Sex
        {
            get;
            set;
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname
        {
            get;
            set;
        }

        public DateTime? BirthDay
        {
            get;
            set;
        }


        public long? LocationId
        {
            get;
            set;
        }

        public Guid? PhotoId
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

        public IEnumerable<ExpandedProperty> ExpandedProperties
        {
            get;
            set;
        }

        public UpdateUser(Guid id)
        {
            _id = id;
        }

        protected override User ExecuteProcedure()
        {
            var user = GetUser();
            if (this.Name != null) user.Name = this.Name;
            if (this.Sex != null) user.Sex = this.Sex.Value;
            if (this.Nickname != null) user.Nickname = this.Nickname;
            if (this.BirthDay != null) user.BirthDay = this.BirthDay.Value;
            if (this.LocationId != null) user.Location = LocationCommon.FindBy(this.LocationId.Value, QueryLevel.None);
            if (this.PhotoId != null) user.Photo = VirtualFileCommon.FindById(this.PhotoId.Value, QueryLevel.None);
            if (this.ExpandedProperties != null) user.ExpandedProperties = this.ExpandedProperties;
            SaveTo(user);


            var account = GetAccount();
            if (this.Email != null) account.Email = this.Email;
            if (this.MobileNumber != null) account.MobileNumber = this.MobileNumber;
            SaveTo(account);

            return user;
        }

        private User GetUser()
        {
            User user = UserCommon.FindById(_id, QueryLevel.Single);
            if (user.IsEmpty()) throw new NotFoundObjectException(typeof(User), _id);
            return user;
        }

        private void SaveTo(User user)
        {
            var repository = Repository.Create<IUserRepository>();
            repository.Update(user);
        }

        private Account GetAccount()
        {
            var account = AccountCommon.FindById(_id, QueryLevel.Single);
            if (account.IsEmpty()) throw new NotFoundObjectException(typeof(Account), _id);
            return account;
        }

        private void SaveTo(Account account)
        {
            var repository = Repository.Create<IAccountRepository>();
            repository.Update(account);
        }
    }
}
