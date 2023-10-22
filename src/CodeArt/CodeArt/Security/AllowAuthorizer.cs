using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.Security
{
    [SafeAccess]
    internal sealed class AllowAuthorizer<T> : IAuthorizer<T> where T : AuthAttribute
    {
        private AllowAuthorizer() { }

        public bool Verify(T attr, DTObject arg)
        {
            return true;
        }

        public static readonly AllowAuthorizer<T> Instance = new AllowAuthorizer<T>();

    }
}
