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
//using RPC.Common;

namespace RPC.Common
{
    [Procedure("UpdatePermission")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class UpdatePermission : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updatePermission", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.EN = arg.EN;
                g.Description = arg.Description;
            });

            return data;
        }
    }
}


