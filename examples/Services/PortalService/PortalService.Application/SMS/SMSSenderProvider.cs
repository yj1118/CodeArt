using System;
using System.Text;
using System.Configuration;

namespace PortalService.Application
{
    public static class SMSSenderProvider
    {
        private const string WinicProvider = "winic";

        public static ISMSSender GetSender(string provider)
        {
            switch (provider)
            {
                case WinicProvider:
                    return (ISMSSender)WinicSender.Instance;
                default:
                    return null;
            }
        }

    }
}
