using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Web.RPC
{
    public interface ILogExtractor
    {
        DTObject GetContent(IProcedure procedure, DTObject arg);
    }
}
