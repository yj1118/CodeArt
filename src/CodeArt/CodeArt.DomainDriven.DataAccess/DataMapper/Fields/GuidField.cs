using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class GuidField : DbField
    {
        public override Type ValueType => typeof(Guid);

        public GuidField(string name)
            : base(name)
        {
        }
    }
}