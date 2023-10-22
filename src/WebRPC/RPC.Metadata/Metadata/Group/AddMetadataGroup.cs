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
    [Procedure("AddMetadataGroup")]
    [SafeAccess()]
    public class AddMetadataGroup : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addMetadataGroup", (g) =>
            {
                g.MetadataId = arg.MetadataId;
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
            });

            return data;
        }

    }
}


