using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.ServiceModel
{
    [SafeAccess]
    internal sealed class AllowAuthFilter : IAuthFilter
    {
        private AllowAuthFilter() { }

        public bool Ignore(string scope, DTObject data)
        {
            return true;
        }

        public static readonly AllowAuthFilter Instance = new AllowAuthFilter();

    }
}
