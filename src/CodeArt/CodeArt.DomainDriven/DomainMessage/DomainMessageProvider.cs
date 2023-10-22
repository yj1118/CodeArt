using CodeArt.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public static class DomainMessageProvider
    {
        private static IDomainMessage _provider;

        public static void Reigster(IDomainMessage provider)
        {
            _provider = provider;
        }

        internal static void Notice(string eventName, DTObject arg, bool sync = false)
        {
            _provider.Notice(eventName, arg, sync);
        }
    }

}
