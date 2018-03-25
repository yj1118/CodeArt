using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.AOP
{
    public interface IAspect
    {
        void Before();

        void After();
    }
}
