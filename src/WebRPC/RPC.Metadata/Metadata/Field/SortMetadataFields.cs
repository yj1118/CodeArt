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
    [Procedure("SortMetadataFields")]
    [SafeAccess()]
    public class SortMetadataFields : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            arg.Transform("items.value=>id");

            var data = ServiceContext.InvokeDynamic("SortMetadataFields", (g) =>
            {
                g.Items = arg.Items;
                g.MetadataId = arg.MetadataId;
                g.GroupId = arg.GroupId;
            });

            return data;
        }

    }
}


