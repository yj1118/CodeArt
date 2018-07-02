using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class DoubleField : DbField
    {
        public override Type ValueType => typeof(double);

        public DoubleField(string name)
            : base(name)
        {
        }
    }
}