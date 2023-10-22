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
    [Procedure("GetMetadataField")]
    [SafeAccess()]
    public class GetMetadataField : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getMetadataField", (g) =>
            {
                g.Id = arg.Id;
                g.MetadataId = arg.MetadataId;
                g.GroupId = arg.GroupId;
            });

            return data;
        }
    }
}
