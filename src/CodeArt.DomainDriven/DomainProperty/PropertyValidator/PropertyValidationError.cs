using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CodeArt.DomainDriven
{
    public class PropertyValidationError : ValidationError
    {
        public string PropertyName
        {
            get;
            internal set;
        }


        internal PropertyValidationError()
        {
        }

        public override void Clear()
        {
            base.Clear();
            this.PropertyName = string.Empty;
        }

    }
}
