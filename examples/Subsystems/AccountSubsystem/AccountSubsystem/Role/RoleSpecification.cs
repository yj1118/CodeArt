using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace AccountSubsystem
{
    [SafeAccess]
    public class RoleSpecification : ObjectValidator<Role>
    {
        public RoleSpecification()
        {

        }

        protected override void Validate(Role obj, ValidationResult result)
        {
            Validator.CheckPropertyRepeated(obj, "MarkedCode", result);
        }
    }
}
