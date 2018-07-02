using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    [SafeAccess]
    public class VirtualFileValidator : ObjectValidator<VirtualFile>
    {
        protected override void Validate(VirtualFile obj, ValidationResult result)
        {

        }
    }
}
