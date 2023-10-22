using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;
using CodeArt.Security;

namespace RPC.Common
{
    [Procedure("AddTenancyMember")]
    [SafeAccess()]
    public class AddTenancyMember : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("addTenancyMember", (g) =>
            {
                //g.SolutionId = AppSession.TenantId;    // todo
                g.AccountName = arg.AccountName;
                g.Password = arg.Password;
                g.RoleIds = arg.RoleIds;
            });

            return data;
        }
    }
}


