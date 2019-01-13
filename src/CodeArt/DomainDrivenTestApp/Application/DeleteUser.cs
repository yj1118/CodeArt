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
    [Service("DeleteUser")]
    public sealed class DeleteUser : ServiceProvider
    {
        protected override DTObject InvokeDynamic(dynamic arg)
        {
            var cmd = new DomainModel.DeleteUser(arg.Id);
            cmd.Execute();
            return DTObject.Empty;
        }
    }
}