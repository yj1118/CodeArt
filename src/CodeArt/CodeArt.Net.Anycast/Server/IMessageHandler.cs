using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public interface IMessageHandler
    {
        void BeginProcess(AnycastServer server, IServerSession origin, Message message, HandlerContext ctx);

        void EndProcess(AnycastServer server, IServerSession origin, Message message, HandlerContext ctx);
    }
}
