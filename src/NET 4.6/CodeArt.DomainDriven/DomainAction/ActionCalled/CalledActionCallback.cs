using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object CalledActionCallback(DomainObject obj, object[] args, object returnValue);
}
