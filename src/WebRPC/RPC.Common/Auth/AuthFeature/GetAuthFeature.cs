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
    [Procedure("GetAuthFeature")]
    [SafeAccess()]
    public class GetAuthFeature : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var data = ServiceContext.InvokeDynamic("getAuthFeature", (g) =>
            {
                g.Id = arg.Id;
            });

            var module = data.GetObject("module");
            data.Transform("!module");
            data.SetValue("Module", module.GetValue<long>("Id"));

            data.Each("ables", (able) =>
            {
                var scope = able.GetObject("scope");
                var permissions = able.GetList("permissions");
                able.Transform("!scope,permissions");

                able.SetValue("Scope", scope.GetValue<long>("Id"));
                var ps = permissions.Select((p) =>
                {
                    var dto = DTObject.Create();
                    dto.SetValue(p.GetValue<long>("Id"));
                    return dto;
                });
                able.SetList("Permissions", ps);
            });

            return data;
        }
    }
}


