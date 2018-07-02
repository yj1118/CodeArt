using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace FileSubsystem
{
    [SafeAccess]
    public sealed class VirtualDirectoryValidator : ObjectValidator<VirtualDirectory>
    {
        public VirtualDirectoryValidator()
        {
        }

        protected override void Validate(VirtualDirectory obj, ValidationResult result)
        {
            if (ExistsSameName(obj))
            {
                result.AddError("VirtualDirectory.NameRepeated", string.Format(Strings.DirectoryExists, obj.Name));
                return;
            }  
        }

        private bool ExistsSameName(VirtualDirectory obj)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            VirtualDirectory dir = repository.FindByName(obj.Name, obj.Parent.Id, QueryLevel.HoldSingle);
            if (dir.IsEmpty()) return false;
            if (dir.Equals(obj)) return false;
            return true;
        }

    }
}
