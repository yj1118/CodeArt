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
    [Procedure("DeleteAuthPlatform")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class DeleteAuthPlatform : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteAuthPlatform", (g) =>
            {

                g.Id = arg.Id; 
            });

            return data;
        }

    }
}


