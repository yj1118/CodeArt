using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public interface IMessageHandler
    {
        void BeginProcess(IServerSession origin, Message message, HandlerContext ctx);

        void EndProcess(IServerSession origin, Message message, HandlerContext ctx);
    }
}
