using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class BooleanField : DbField
    {
        public override Type ValueType => typeof(bool);

        public BooleanField(string name)
            : base(name)
        {
        }
    }
}