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
    [Procedure("DeleteAuthScope")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class DeleteAuthScope : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteAuthScope", (g) =>
            {

                g.Id = arg.Id; 
            });

            return data;
        }

    }
}


