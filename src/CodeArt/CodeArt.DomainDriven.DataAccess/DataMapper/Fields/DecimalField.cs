using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class DecimalField : DbField
    {
        public override Type ValueType => typeof(decimal);

        public DecimalField(string name)
            : base(name)
        {
        }
    }
}