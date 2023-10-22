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
    [Procedure("AddMetadata")]
    [SafeAccess()]
    public class AddMetadata : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
        
            var data = ServiceContext.InvokeDynamic("addMetadata", (g) =>
            {
                g.MarkedCode = arg.MarkedCode;
            });

            return data;
        }

    }
}


