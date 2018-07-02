using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace LocationSubsystem
{
    public class UpdateLocation : Command<Location>
    {
        private long _id;

        public string Name
        {
            get;
            set;
        }

        public int? SortNumber
        {
            get;
            set;
        }

        public string MarkedCode
        {
            get;
            set;
        }

        public UpdateLocation(long id)
        {
            _id = id;
        }

        protected override Location ExecuteProcedure()
        {
            Location location = LocationCommon.FindBy(_id, QueryLevel.Single);
            SetLocation(location);
            SaveTo(location);
            return location;
        }

        private void SetLocation(Location location)
        {
            if (this.Name != null) location.Name = this.Name;
            if (this.SortNumber != null) location.SortNumber = this.SortNumber.Value;
            if (this.MarkedCode != null) location.MarkedCode = this.MarkedCode;
        }

        private void SaveTo(Location location)
        {
            ILocationRepository repository = Repository.Create<ILocationRepository>();
            repository.Update(location);
        }
    }

}
