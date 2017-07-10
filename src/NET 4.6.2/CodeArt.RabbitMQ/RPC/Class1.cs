using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyNetQ;


namespace CodeArt.RabbitMQ
{
    public static class Test
    {
        public static void Invoke()
        {
            var bus = RabbitHutch.CreateBus("host = myServer; virtualHost = myVirtualHost; username = mike; password = topsecret");
            
        }
    }


}
