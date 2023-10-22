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
using CodeArt.AppSetting;

namespace RPC.Common
{
    [Procedure("AddRole")]
    [SafeAccess()]

    public class AddRole : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addRole", (g) =>
            {
                g.Name = arg.Name;
                g.PlatformId = arg.PlatformId;
                g.PermissionCodes = arg.PermissionCodes;
                g.IsSystem = true; //后台创建的默认就是系统角色
                g.IsPublic = arg.GetValue<int>("IsPublic") == 1 ? true : false;
                g.MarkedCode = arg.MarkedCode;
                g.Description = arg.Description;
            });

            return data;
        }

    }
}


