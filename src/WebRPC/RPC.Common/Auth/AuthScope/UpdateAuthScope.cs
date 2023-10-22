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
    [Procedure("UpdateAuthScope")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class UpdateAuthScope : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateAuthScope", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.EN = arg.EN;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
            });


            return data;
        }
    }
}


