using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public interface IMessageListener
    {
        void Process(AnycastClient client, Message message);
    }
}
