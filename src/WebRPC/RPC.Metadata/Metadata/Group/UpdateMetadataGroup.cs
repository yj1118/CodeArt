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
    [Procedure("UpdateMetadataGroup")]
    [SafeAccess()]
    public class UpdateMetadataGroup : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateMetadataGroup", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
                g.MetadataId = arg.MetadataId;
            });

            return data;
        }
    }
}


