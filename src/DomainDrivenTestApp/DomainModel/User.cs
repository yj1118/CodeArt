using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.DomainDriven;

namespace DomainDrivenTestApp.DomainModel
{
    [Remotable("{id,name,wife,son}")]
    [ObjectRepository(typeof(IUserRepository))]
    public class User : AggregateRoot<User, int>
    {
        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, User>("Name");

        /// <summary>
        /// 用户名称
        /// </summary>
        [PropertyRepository]
        [StringLength(1, 20)]
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

        private static readonly DomainProperty WifeProperty = DomainProperty.Register<User, User>("Wife");

        /// <summary>
        /// 妻子
        /// </summary>
        [PropertyRepository]
        public User Wife
        {
            get
            {
                return GetValue<User>(WifeProperty);
            }
            set
            {
                SetValue(WifeProperty, value);
            }
        }

        private static readonly DomainProperty SonProperty = DomainProperty.Register<User, User>("Son");

        /// <summary>
        /// 儿子
        /// </summary>
        [PropertyRepository]
        public User Son
        {
            get
            {
                return GetValue<User>(SonProperty);
            }
            set
            {
                SetValue(SonProperty, value);
            }
        }

        private class UserEmpty : User
        {
            public UserEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly User Empty = new UserEmpty();



        [ConstructorRepository]
        public User(int id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}