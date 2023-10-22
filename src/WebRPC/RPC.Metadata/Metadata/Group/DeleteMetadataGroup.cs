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
    [Procedure("DeleteMetadataGroup")]
    [SafeAccess()]
    public class DeleteMetadataGroup : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteMetadataGroup", (g) =>
            {
                g.Id = arg.Id;
                g.MetadataId = arg.MetadataId;
            });

            return data;
        }

    }
}


