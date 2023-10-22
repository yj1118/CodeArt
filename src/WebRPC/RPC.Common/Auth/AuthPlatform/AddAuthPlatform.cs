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
    [Procedure("AddAuthPlatform")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class AddAuthPlatform : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addAuthPlatform", (g) =>
            {
                g.Name = arg.Name;

                g.EN = arg.EN;
                g.Description = arg.Description;
            });

            return data;
        }

    }
}


