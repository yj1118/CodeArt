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
    [Procedure("UpdatePlugin")]
    [SafeAccess()]
    public class UpdatePlugin : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updatePlugin", (g) =>
            {
                g.Id = arg.Id;
                g.CategoryId = arg.CategoryId;
                g.TagIds = arg.TagIds;
                g.Description = arg.Description;
            });

            return data;
        }
    }
}


