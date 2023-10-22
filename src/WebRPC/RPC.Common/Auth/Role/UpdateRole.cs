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

namespace RPC.Common
{
    [Procedure("UpdateRole")]
    [SafeAccess()]
 
    public class UpdateRole : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("updateRole", (g) =>
            {
                g.Id = arg.Id;
                g.Name = arg.Name;
                if (arg.Exist("IsPublic")) g.IsPublic = arg.GetValue<int>("IsPublic") == 1 ? true : false;
                g.PermissionCodes = arg.PermissionCodes;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
            });

            return data;
        }
    }
}


