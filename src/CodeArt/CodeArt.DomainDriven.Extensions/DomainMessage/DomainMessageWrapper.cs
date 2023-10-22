using CodeArt.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    internal class DomainMessageWrapper : IDomainMessage
    {
        private DomainMessageWrapper() { }

        public void Notice(string eventName, DTObject arg, bool sync = false)
        {
            DomainMessage.Notice(eventName, arg, sync);
        }

        public static readonly DomainMessageWrapper Instance = new DomainMessageWrapper();
    }
}
