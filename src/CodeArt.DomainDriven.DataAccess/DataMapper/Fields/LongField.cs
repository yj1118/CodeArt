using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class LongField : DbField
    {
        public override Type ValueType => typeof(long);

        public LongField(string name)
            : base(name)
        {
        }
    }
}