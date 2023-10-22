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
    [Procedure("DeleteTenancyMember")]
    [SafeAccess()]
    public class DeleteTenancyMember : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("deleteTenancyMember", (g) =>
            {
                g.UserId = arg.UserId;
                // g.SolutionId = AppSession.TenantId; todo
            });

            return data;
        }
    }
}


