using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.RPC
{
    public interface IServerFactory
    {
        IServer Create(string method);

        IEnumerable<IServer> GetAll();
    }
}
