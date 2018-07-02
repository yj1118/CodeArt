﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using UserSubsystem;
using CodeArt.DomainDriven;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("deleteUsers")]
    public class DeleteUsers : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new UserSubsystem.DeleteUsers(arg.Ids.OfType<Guid>());
            cmd.Execute();
            return DTObject.Empty;
        }
    }
}