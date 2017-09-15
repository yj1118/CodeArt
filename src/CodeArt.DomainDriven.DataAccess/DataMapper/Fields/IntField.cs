using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class IntField : DbField
    {
        public override Type ValueType => typeof(int);

        public IntField(string name)
            : base(name)
        {
        }
    }
}