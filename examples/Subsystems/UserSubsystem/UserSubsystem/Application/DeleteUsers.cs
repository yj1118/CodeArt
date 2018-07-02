using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using AccountSubsystem;

namespace UserSubsystem
{
    public class DeleteUsers : Command
    {
        private IEnumerable<Guid> _userIds;

        public DeleteUsers(IEnumerable<Guid> userIds)
        {
            _userIds = userIds;
        }

        protected override void ExecuteProcedure()
        {
            IUserRepository repository = Repository.Create<IUserRepository>();
            foreach(var id in _userIds)
            {
                var user = repository.Find(id, QueryLevel.Mirroring);
                repository.Delete(user);
            }
        }

    }

    public class DeleteUser : Command
    {
        private Guid _userId;

        public DeleteUser(Guid userId)
        {
            _userId = userId;
        }

        protected override void ExecuteProcedure()
        {
            IUserRepository repository = Repository.Create<IUserRepository>();
            var user = repository.Find(_userId, QueryLevel.None);
            if (user.IsEmpty()) return;
            repository.Delete(user);
        }

    }

}
