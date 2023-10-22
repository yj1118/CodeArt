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
    [Procedure("UpdateMetadataField")]
    [SafeAccess()]
    public class UpdateMetadataField : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateMetadataField", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                g.EN = arg.EN;
                g.MetadataId = arg.MetadataId;
                g.GroupId = arg.GroupId;
                g.Description = arg.Description;
                g.Algorithm = arg.Algorithm;
                g.Unit = arg.Unit;
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


