using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    public interface IRtpModuleFactory
    {
        IEnumerable<RtpModule> Create();
    }
}