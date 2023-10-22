using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using RPC.Common;
using CodeArt.ServiceModel;
using CodeArt;

namespace RPC.Common
{
    [Procedure("SignInOrRegister", typeof(LogExtractor))]
    [Procedure("LoginOrRegister", typeof(LogExtractor))]
    [SafeAccess()]
    public class SignInOrRegister : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var result = AuthUtil.SignInOrRegister(arg);

            return result;
        }
    }
}