using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Metadata
{
    [Procedure("GetMetadataGroup")]
    [SafeAccess()]
    public class GetMetadataGroup : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getMetadataGroup", (g) =>
            {
                g.Id = arg.Id;
                g.MetadataId = arg.MetadataId;
            });

            return data;
        }
    }
}
