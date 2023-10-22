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
    [Procedure("AddMetadataField")]
    [SafeAccess()]
    public class AddMetadataField : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addMetadataField", (g) =>
            {
                g.GroupId = arg.GroupId;
                g.MetadataId = arg.MetadataId;
                g.Name = arg.Name;
                g.EN = arg.EN;
                g.Description = arg.Description;
                g.Unit = arg.Unit;
                g.Algorithm = arg.Algorithm;
                g.Min = arg.Min;
                g.Max = arg.Max;
                g.Step = arg.Step;
                g.Type = arg.Type;
                g.Required = arg.Required;
                g.RoleIds = arg.RoleIds;
                g.Options = arg.Options;
            });

            return data;
        }

    }
}


