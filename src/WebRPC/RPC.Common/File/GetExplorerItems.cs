using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("GetExplorerItems")]
    [SafeAccess()]
    public class GetExplorerItems : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            return FileServiceUtil.GetExplorerItems(arg);
        }

    }
}


