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
    [Procedure("GetTenantMember")]
    [SafeAccess()]
    public class GetTenantMember : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            return TenantUtil.GetTenantMemberDetail(arg);
        }
    }
}


