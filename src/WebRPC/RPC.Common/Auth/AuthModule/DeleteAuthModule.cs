using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("DeleteAuthModule")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class DeleteAuthModule : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteAuthModule", (g) =>
            {
                g.Id = arg.Id; 
            });

            return data;
        }

    }
}


