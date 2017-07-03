using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 行为过程
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    public delegate object ActionProcedure(DomainObject obj, params object[] args);
}