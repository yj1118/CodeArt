using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.ServiceModel;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.DomainDriven;

namespace DomainDrivenTestApp
{
    [SafeAccess]
    [Service("UpdateUser")]
    public sealed class UpdateUser : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new DomainModel.UpdateUser(arg.Id);
            cmd.Name = arg.Name;
            cmd.Execute();
            return DTObject.Empty;
        }
    }
}