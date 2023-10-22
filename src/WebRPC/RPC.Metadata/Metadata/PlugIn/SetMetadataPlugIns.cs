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
    [Procedure("SetMetadataPlugIns")]
    [SafeAccess()]
    public class SetMetadataPlugIns : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("SetMetadataPlugIns", (g) =>
            {
                g.GroupId = arg.GroupId;
                g.MetadataId = arg.MetadataId;
                g.PlugIns = arg.PlugIns;
            });

            return data;
        }
    }
}


