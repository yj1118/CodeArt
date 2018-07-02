using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using AccountSubsystem;
using LocationSubsystem;
using FileSubsystem;

namespace UserSubsystem
{
    /// <summary>
    /// 创建用户
    /// </summary>
    public class CreateUser : Command<User>
    {
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
        public string NickName
        {
            get;
            set;
        }

        public string MobileNumber
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public Guid[] RoleIds
        {
            get;
            set;
        }

        public IEnumerable<ExpandedProperty> ExpandedProperties
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

        public bool? IsEnabled
        {
            get;
            set;
        }

        private string _accountName;
        private string _password;
        private long _diskSize;

        public CreateUser(string accountName, string password, long diskSize)
        {
            _accountName = accountName;
            _password = password;
            _diskSize = diskSize;
        }

        protected override User ExecuteProcedure()
        {
            var user = BuildUser();
            SaveTo(user);
            return user;
        }

        private User BuildUser()
        {
            var account = OpenAccount();
            var disk = OpenDisk(account);

            var user = new User(account, disk)
            {
                Name = this.Name ?? string.Empty,
                Nickname = this.NickName ?? string.Empty,
                Sex = this.Sex ?? UserSubsystem.Sex.Secret,
                ExpandedProperties = this.ExpandedProperties ?? Array.Empty<ExpandedProperty>()
            };
            if(this.LocationId != null) user.Location = LocationCommon.FindBy(this.LocationId.Value, QueryLevel.None);
            if (this.PhotoId !=null) user.Photo = VirtualFileCommon.FindById(this.PhotoId.Value, QueryLevel.None);
            return user;
        }

        /// <summary>
        /// 开通账号
        /// </summary>
        /// <returns></returns>
        private Account OpenAccount()
        {
            var cmd = new CreateAccount(_accountName, _password, this.RoleIds)
            {
                Email = this.Email??string.Empty,
                MobileNumber = this.MobileNumber ?? string.Empty,
                IsEnabled = this.IsEnabled
            };
            return cmd.Execute();
        }

        /// <summary>
        /// 开通磁盘空间
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private VirtualDisk OpenDisk(Account account)
        {
            var cmd = new CreateVirtualDisk(account.Id, _diskSize);
            return cmd.Execute();
        }

        private void SaveTo(User user)
        {
            var repository = Repository.Create<IUserRepository>();
            repository.Add(user);
        }
    }
}
