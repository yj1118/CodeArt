using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Web.WebPages;

namespace RPC.Common
{
    [Procedure("SignOut", typeof(LogExtractor))]
    [Procedure("Logout", typeof(LogExtractor))]
    [SafeAccess()]
    public class SignOut : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            return DTObject.Empty;
        }
    }
}