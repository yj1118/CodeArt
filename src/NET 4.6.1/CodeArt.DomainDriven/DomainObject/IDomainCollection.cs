using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


using CodeArt.Util;
using CodeArt.Runtime;


namespace CodeArt.DomainDriven
{
    public interface IDomainCollection

    {
        DomainObject Parent
        {
            get;
            set;
        }
    }
}