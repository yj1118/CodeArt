using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// 用户编号就是账号编号，也是虚拟磁盘的编号
    /// </summary>
    [Remotable()]
    [ObjectRepository(typeof(IUserRepository))]
    [ObjectValidator(typeof(UserValidator))]
    public class User : AggregateRoot<User, Guid>
    {
        [PropertyRepository()]
        [StringLength(0, 25)]
        public static readonly DomainProperty NicknameProperty = DomainProperty.Register<string, User>("Nickname");

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Nickname
        {
            get
            {
                return GetValue<string>(NicknameProperty);
            }
            set
            {
                SetValue(NicknameProperty, value);
            }
        }

        [PropertyRepository()]
        [StringLength(0, 25)]
        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, User>("Name");

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        [PropertyRepository()]
        public static readonly DomainProperty SexProperty = DomainProperty.Register<Sex, User>("Sex", Sex.Secret);

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex
        {
            get
            {
                return GetValue<Sex>(SexProperty);
            }
            set
            {
                SetValue(SexProperty,value);
            }
        }

        [PropertyRepository()]
        public static readonly DomainProperty BirthDayProperty = DomainProperty.Register<Emptyable<DateTime>, User>("BirthDay");

        /// <summary>
        /// 用户生日
        /// </summary>
        public Emptyable<DateTime> BirthDay
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(BirthDayProperty);
            }
            set
            {
                SetValue(BirthDayProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty LocationProperty = DomainProperty.Register<Location, User>("Location");

        /// <summary>
        /// 用户所在地
        /// </summary>
        public Location Location
        {
            get
            {
                return GetValue<Location>(LocationProperty);
            }
            set
            {
                SetValue(LocationProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty PhotoProperty = DomainProperty.Register<VirtualFile, User>("Photo");

        /// <summary>
        /// 用户照片
        /// </summary>
        public VirtualFile Photo
        {
            get
            {
                return GetValue<VirtualFile>(PhotoProperty);
            }
            set
            {
                SetValue(PhotoProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        [NotEmpty]
        public static readonly DomainProperty AccountProperty = DomainProperty.Register<Account, User>("Account");

        /// <summary>
        /// 用户的账号信息
        /// </summary>
        public Account Account
        {
            get
            {
                return GetValue<Account>(AccountProperty);
            }
            set
            {
                SetValue(AccountProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty DiskProperty = DomainProperty.Register<VirtualDisk, User>("Disk");

        /// <summary>
        /// 用户的磁盘空间信息
        /// </summary>
        public VirtualDisk Disk
        {
            get
            {
                return GetValue<VirtualDisk>(DiskProperty);
            }
            set
            {
                SetValue(DiskProperty, value);
            }
        }

        #region 扩展信息

        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty ExpandedPropertiesProperty = DomainProperty.RegisterCollection<ExpandedProperty, User>("ExpandedProperties");

        private DomainCollection<ExpandedProperty> _ExpandedProperties
        {
            get
            {
                return GetValue<DomainCollection<ExpandedProperty>>(ExpandedPropertiesProperty);
            }
            set
            {
                SetValue(ExpandedPropertiesProperty, value);
            }
        }

        /// <summary>
        /// 子地理位置
        /// </summary>
        public IEnumerable<ExpandedProperty> ExpandedProperties
        {
            get
            {
                return _ExpandedProperties;
            }
            set
            {
                _ExpandedProperties = new DomainCollection<ExpandedProperty>(ExpandedPropertiesProperty, value);
            }
        }

        #endregion


        #region 用户的账户相关信息

        public string MobileNumber
        {
            get
            {
                return this.Account.MobileNumber;
            }
        }

        public string Email
        {
            get
            {
                return this.Account.Email;
            }
        }

        public string AccountName
        {
            get
            {
                return this.Account.Name;
            }
        }

        public DateTime RegisterTime
        {
            get
            {
                return this.Account.CreateTime;
            }
        }

        
        #endregion

     
        public string DisplayName
        {
            get
            {
                return !string.IsNullOrEmpty(this.Nickname) ? this.Nickname :
                    (!string.IsNullOrEmpty(this.Name) ? this.Name : this.Account.Name);
            }
        }

        /// <summary>
        /// 每一个用户都会有一个账号和一个虚拟磁盘
        /// </summary>
        /// <param name="account"></param>
        /// <param name="disk"></param>
        public User(Account account, VirtualDisk disk)
            : base(account.Id)
        {
            this.Account = account;
            this.Disk = disk;
            this.OnConstructed();
        }


        [ConstructorRepository]
        public User(Guid id)
            : base(id)
        {
        }

        public override void OnDeleted()
        {
            {
                var cmd = new DeleteAccount(this.Account.Id);
                cmd.Execute();
            }
            {
                var cmd = new DeleteVirtualDisk(this.Disk.Id);
                cmd.Execute();
            }
            base.OnDeleted();
        }

        private class UserEmpty : User
        {
            public UserEmpty()
                : base(Account.Empty,VirtualDisk.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly User Empty = new UserEmpty();
    }

    public enum Sex : byte
    {
        Secret = 0,
        Male = 1,
        Female = 2
    }

}
