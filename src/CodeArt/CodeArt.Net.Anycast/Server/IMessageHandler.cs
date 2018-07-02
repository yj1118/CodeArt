using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public interface IMessageHandler
    {
        void Process(IServerSession origin, Message message);
    }
}
