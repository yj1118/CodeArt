using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class ByteField : DbField
    {
        public override Type ValueType => typeof(byte);

        public ByteField(string name)
            : base(name)
        {
        }
    }
}