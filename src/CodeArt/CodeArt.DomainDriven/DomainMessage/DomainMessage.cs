using CodeArt.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IDomainMessage
    {
        void Notice(string eventName, DTObject arg, bool sync = false);
    }

}
