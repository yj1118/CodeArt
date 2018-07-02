using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace LocationSubsystem
{
    public sealed class CreateLocation : Command<Location>
    {
        private string _name;

        public string MarkedCode
        {
            get;
            set;
        }

        public int? SortNumber
        {
            get;
            set;
        }

        public long? ParentId
        {
            get;
            set;
        }

        public CreateLocation(string name)
        {
            _name = name;
        }

        protected override Location ExecuteProcedure()
        {
            Location location = BuildLocation();
            var repository = Repository.Create<ILocationRepository>();
            repository.Add(location);
            return location;
        }

        private Location BuildLocation()
        {
            var id = DataPortal.GetIdentity<Location>();

            Location location = new Location(id)
            {
                Name = _name,
                SortNumber = this.SortNumber ?? 0,
                MarkedCode = this.MarkedCode ?? string.Empty
            };
            SetParent(location);
            return location;
        }

        private void SetParent(Location location)
        {
            if (this.ParentId != null)
            {
                var parent = LocationCommon.FindBy(this.ParentId.Value, QueryLevel.Single);
                if (!parent.IsEmpty()) location.Parent = parent;
            }
        }
    }
}
