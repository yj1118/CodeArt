using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainEventTest
{
    public class User
    {
        public long Id
        {
            get;
            private set;
        }

        public decimal Points
        {
            get;
            set;
        }

        /// <summary>
        /// 赠送了多少碎片
        /// </summary>
        public decimal Pieces
        {
            get;
            set;
        }

        /// <summary>
        /// 收到了多少礼物
        /// </summary>
        public decimal Gift
        {
            get;
            set;
        }


        public User(long id)
        {
            this.Id = id;
            this.Points = 0;
            this.Pieces = 0;
        }

        private static Dictionary<long, User> _users = new Dictionary<long, User>();

        public static User GetOrCreate(long id)
        {
            User user = null;
            if (!_users.TryGetValue(id, out user))
            {
                lock(_users)
                {
                    if (!_users.TryGetValue(id, out user))
                    {
                        user = new User(id);
                        _users.Add(id, user);
                    }
                }
            }

            return user;
        }


        private static object _syncObject = new object();

        /// <summary>
        /// 独占使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        public static void Single(long id, Action<User> action)
        {
            var user = GetOrCreate(id);
            lock (_syncObject)
            {
                action(user);
            }
        }

    }
}
