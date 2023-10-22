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
    [Procedure("SetDataInstance")]
    [SafeAccess()]
    public class SetDataInstance : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("setDataInstance", (g) =>
            {
                g.Id = arg.Id;
                g.MetadataCode = arg.MetadataCode;
                g.Fields = arg.Fields;
            });

            return data;
        }
    }
}


