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
    [Procedure("UpdateTenancyRole")]
    [SafeAccess()]
 
    public class UpdateTenancyRole : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateTenancyRole", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
                g.PermissionCodes = arg.PermissionCodes;
                g.Description = arg.Description;
            });

            return data;
        }
    }
}


