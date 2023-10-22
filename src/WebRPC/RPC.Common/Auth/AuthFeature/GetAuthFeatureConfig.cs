using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    [Procedure("GetAuthFeatureConfig")]
    [SafeAccess()]
    public class GetAuthFeatureConfig : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var dto = DTObject.Create();
            GetModules(dto, arg);
            GetScopes(dto);
            GetPermissions(dto);
            return dto;
        }

        private static void GetModules(DTObject dto, dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAuthModules", (g) =>
            {
                g.PlatformId = arg.platformId;
                g.BelongValue = arg.BelongValue;
                g.Slim = true;
            });
            data.Transform("rows.id=>value;rows.name=>text");

            dto.SetList("Modules", data.GetList("rows", true));
        }

        private static void GetScopes(DTObject dto)
        {
            var data = ServiceContext.InvokeDynamic("getAuthScopes", (g) =>
            {
                g.Slim = true;
            });
            data.Transform("rows.id=>value;rows.name=>text");

            dto.SetList("Scopes", data.GetList("rows", true));
        }

        private static void GetPermissions(DTObject dto)
        {
            var data = ServiceContext.InvokeDynamic("getPermissions", (g) =>
            {
                g.Slim = true;
            });
            data.Transform("rows.id=>value;rows.name=>text");

            dto.SetList("Permissions", data.GetList("rows", true));
        }
    }
}


