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
    [Procedure("GetMetadatas")]
    [SafeAccess()]
    public class GetMetadatas : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getMetadatas", (g) =>
            {
                g.Sort = arg.Sort;
                g.Slim = arg.Slim;
                g.MarkedCodes = arg.MarkedCodes;
            });

            if (arg.Sort != null)
            {
                data.Transform("rows.id=>value;rows.name=>text");
            }

            return data;
        }
    }
}


