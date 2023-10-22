using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.TestPlatform
{
    public interface IVariableProvider
    {
        DTObject Get(string package, string name);
    }
}
