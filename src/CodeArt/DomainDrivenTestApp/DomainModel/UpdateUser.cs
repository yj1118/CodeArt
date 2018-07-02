using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace DomainDrivenTestApp.DomainModel
{
    public class UpdateUser : Command<User>
    {
        private int _id;

        public string Name
        {
            get;
            set;
        }

        public UpdateUser(int id)
        {
            _id = id;
        }

        protected override User ExecuteProcedure()
        {
            var repository = Repository.Create<IUserRepository>();
            var user = repository.Find(_id, QueryLevel.Single);
            if (this.Name != null) user.Name = this.Name;

            repository.Update(user);

            return user;
        }
    }
}
