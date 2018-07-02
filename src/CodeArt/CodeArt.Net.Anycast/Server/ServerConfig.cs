using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public class ServerConfig
    {
        public bool IsSsl
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public IAuthenticator Authenticator
        {
            get;
            set;
        }

        public ServerConfig()
        {
            this.IsSsl = false;
            this.Port = 8007;
            this.Authenticator = UnlimitedAuthenticator.Instance;
        }
    }
}