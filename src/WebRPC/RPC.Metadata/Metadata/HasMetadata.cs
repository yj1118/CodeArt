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

namespace RPC.Metadata
{
    [Procedure("HasMetadata")]
    [SafeAccess()]
    public class HasMetadata : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("hasMetadata", (g) =>
            {
                g.MarkedCode = arg.MarkedCode;
            });

            return data;
        }

    }
}


