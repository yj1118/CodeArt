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
    [Procedure("SortMetadataGroups")]
    [SafeAccess()]
    public class SortMetadataGroups : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            arg.Transform("items.value=>id");

            var data = ServiceContext.InvokeDynamic("sortMetadataGroups", (g) =>
            {
                g.Items = arg.Items;
                g.MetadataId = arg.MetadataId;
            });

            return data;
        }

    }
}


