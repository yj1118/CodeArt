using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.DTO;
using CodeArt.AppSetting;
using CodeArt.Security;

namespace RPC.Common
{
    [SafeAccess]
    public sealed class AuthFilter : IAuthFilter
    {
        private AuthFilter() { }

        public bool Ignore(string scope, DTObject data)
        {
            switch (scope)
            {
                case "global": return true;  //忽略，不验证
                case "my": return false; //不忽略，需要验证
            }

            return true;
        }

        public readonly static AuthFilter Instance = new AuthFilter();
    }
}
