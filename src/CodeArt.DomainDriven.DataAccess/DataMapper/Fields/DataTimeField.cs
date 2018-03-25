using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class DataTimeField : DbField
    {
        public override Type ValueType => typeof(DateTime);

        public DataTimeField(string name)
            : base(name)
        {
        }
    }
}