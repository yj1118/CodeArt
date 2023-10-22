﻿using System;
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
    [Procedure("AddAuthFeature")]
    [SafeAccess()]
    [Identity(Role = RoleCodes.sa)]
    public class AddAuthFeature : Procedure
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            arg.Transform("module=>moduleId");
            arg.Transform("ables.scope=>scopeId;ables.permissions=>permissionIds");

            var data = ServiceContext.InvokeDynamic("addAuthFeature", (g) =>
            {
                g.ModuleId = arg.ModuleId;
                g.Name = arg.Name;
                g.EN = arg.EN;
                g.Ables = arg.Ables;
            });

            return data;
        }

    }
}


