using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CodeArt.DomainDriven.DataAccess
{
    public abstract class DbField
    {
        public string Name
        {
            get;
            private set;
        }

        public abstract Type ValueType
        {
            get;
        }

        public DbField(string name)
        {
            this.Name = name;
        }
    }
}
