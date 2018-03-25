using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class ShortField : DbField
    {
        public override Type ValueType => typeof(short);

        public ShortField(string name)
            : base(name)
        {
        }
    }
}