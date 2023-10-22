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
    [Procedure("GetMetadataPlugIn")]
    [SafeAccess()]
    public class GetMetadataPlugIn : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getMetadataPlugIn", (g) =>
            {
                g.Id = arg.Id;
                g.MarkedCode = arg.MarkedCode;
                g.GroupId = arg.GroupId;
            });

            return data;
        }


    }
}


