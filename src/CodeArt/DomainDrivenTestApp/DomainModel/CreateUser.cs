using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace DomainDrivenTestApp.DomainModel
{
    public class CreateUser : Command<User>
    {
        private int _id;
        private string _name;

        private int _wifeId;
        private int _sonId;

        public CreateUser(int id, string name,int wifeId,int sonId)
        {
            _id = id;
            _name = name;
            _wifeId = wifeId;
            _sonId = sonId;
        }

        protected override User ExecuteProcedure()
        {
            var repository = Repository.Create<IUserRepository>();

            User wife = User.Empty;
            if (_wifeId > 0)
            {
                wife = repository.Find(_wifeId, QueryLevel.None);
            }

            User son = User.Empty;
            if (_sonId > 0)
            {
                son = repository.Find(_sonId, QueryLevel.None);
            }


            User user = new User(_id)
            {
                Name = _name,
                Wife = wife,
                Son = son
            };

            repository.Add(user);

            return user;
        }
    }
}
