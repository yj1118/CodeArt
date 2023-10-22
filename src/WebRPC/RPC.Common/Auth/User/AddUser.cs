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
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("AddUser", typeof(LogExtractor))]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class AddUser : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var result = ServiceContext.InvokeDynamic("AddUser", (g) =>
            {
                g.AccountName = arg.AccountName;
                g.Password = arg.Password;
            });

            return result;
        }
    }
}