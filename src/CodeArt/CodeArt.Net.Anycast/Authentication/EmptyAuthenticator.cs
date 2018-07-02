using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 无限制的认证器
    /// </summary>
    public sealed class UnlimitedAuthenticator : IAuthenticator
    {
        private UnlimitedAuthenticator() { }

 
        public CertifiedResult Check(DTObject identity)
        {
            return CertifiedResult.Success;
        }

        public readonly static UnlimitedAuthenticator Instance = new UnlimitedAuthenticator();

    }
}
