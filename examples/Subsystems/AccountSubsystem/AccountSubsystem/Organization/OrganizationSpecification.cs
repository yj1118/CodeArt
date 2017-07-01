using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;

namespace AccountSubsystem
{
    [SafeAccess]
    internal sealed class OrganizationSpecification : ObjectValidator<Organization>
    {
        public OrganizationSpecification()
        {
        }

        protected override void Validate(Organization obj, ValidationResult result)
        {
        }
    }
}
