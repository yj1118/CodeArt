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
    [Procedure("DeleteMetadataField")]
    [SafeAccess()]
    public class DeleteMetadataField : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteMetadataField", (g) =>
            {
                g.Id = arg.Id;
                g.MetadataId = arg.MetadataId;
                g.GroupId = arg.GroupId;
            });

            return data;
        }

    }
}


