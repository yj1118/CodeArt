using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class FloatField : DbField
    {
        public override Type ValueType => typeof(float);

        public FloatField(string name)
            : base(name)
        {
        }
    }
}