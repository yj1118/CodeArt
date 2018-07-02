using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace DomainDrivenTestApp.DomainModel
{
    public class DeleteUser : Command
    {
        private int _id;

        public DeleteUser(int id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IUserRepository>();
            var user = repository.Find(_id, QueryLevel.Single);

            if (user.IsEmpty())
                return;

            repository.Delete(user);
        }
    }
}
