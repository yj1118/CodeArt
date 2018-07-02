using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace LocationSubsystem
{
    public static class LocationCommon
    {
        public static Location FindBy(long id, QueryLevel level)
        {
            var repository = Repository.Create<ILocationRepository>();
            return repository.Find(id, level);
        }


        internal static Location FindBy(string markedCode, QueryLevel level)
        {
            if (string.IsNullOrEmpty(markedCode)) return Location.Empty;
            var repository = Repository.Create<ILocationRepository>();
            return repository.FindBy(markedCode, level);
        }

        internal static IEnumerable<Location> FindChilds(int parentId, QueryLevel level)
        {
            var repository = Repository.Create<ILocationRepository>();
            return repository.FindChilds(parentId, level);
        }
    }
}
