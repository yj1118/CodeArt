using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace AccountSubsystem
{
    [SafeAccess]
    internal sealed class PermissionSpecification : ObjectValidator<Permission>
    {
        public PermissionSpecification()
        {

        }

        protected override void Validate(Permission obj, ValidationResult result)
        {
            Validator.CheckPropertyRepeated(obj, Permission.NameProperty, result);
            Validator.CheckPropertyRepeated(obj, Permission.MarkedCodeProperty, result);
        }
    }
}
