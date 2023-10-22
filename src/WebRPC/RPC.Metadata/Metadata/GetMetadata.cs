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
    [Procedure("GetMetadata")]
    [SafeAccess()]
    public class GetMetadata : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getMetadata", (g) =>
            {
                g.MarkedCode = arg.MarkedCode;
                g.Id = arg.Id;
                g.GroupId = arg.GroupId;
                g.SortField = arg.SortField;
                g.Slim = arg.Slim;
                g.Group = arg.Group;
                g.SortGroup = arg.SortGroup;
                g.GroupMarkedCode = arg.GroupMarkedCode;
            });

            if (arg.SortField != null)
            {
                data.Transform("fields=>rows");
                data.Transform("rows.id=>value;rows.name=>text");
            }

            if (arg.SortGroup != null)
            {
                data.Transform("groups=>rows");
                data.Transform("rows.id=>value;rows.name=>text");
            }

            return data;
        }


    }
}


