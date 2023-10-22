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
    [Procedure("SignIn", typeof(LogExtractor))]
    [Procedure("Login", typeof(LogExtractor))]
    [SafeAccess()]
    public class SignIn : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var result = AuthUtil.SignIn(arg);

            return result;
        }
    }
}