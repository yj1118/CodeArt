using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven.DataAccess
{
    public class StringField : DbField
    {
        public int MaxLength
        {
            get;
            private set;
        }

        public bool ASCII
        {
            get;
            private set;
        }

        public override Type ValueType => typeof(string);

        public StringField(string name,int maxLength,bool ascii)
            : base(name)
        {
            this.MaxLength = maxLength;
            this.ASCII = ascii;
        }
    }
}