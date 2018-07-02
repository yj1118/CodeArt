using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace PortalService.Application
{
    public interface ISMSSender
    {
        void Send(string mobileNumber, string message);
    }
}
