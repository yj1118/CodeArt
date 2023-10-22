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
    [Procedure("DeleteUsers")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class DeleteUsers : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteUsers", (g) =>
            {
                g.Ids = arg.Ids; 
            });

            return data;
        }

    }
}


