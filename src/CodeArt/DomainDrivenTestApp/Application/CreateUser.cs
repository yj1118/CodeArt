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
    [Service("CreateUser")]
    public sealed class CreateUser : ServiceProvider
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var cmd = new DomainModel.CreateUser(arg.Id, arg.Name, arg.WifeId ?? 0, arg.SonId ?? 0);
            cmd.Execute();

            return DTObject.Empty;
        }
    }
}