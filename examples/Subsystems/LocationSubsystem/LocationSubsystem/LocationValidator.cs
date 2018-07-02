using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace LocationSubsystem
{
    [SafeAccess]
    internal sealed class LocationValidator : ObjectValidator<Location>
    {
        public LocationValidator()
        {

        }

        protected override void Validate(Location obj, ValidationResult result)
        {
            Validator.CheckPropertyRepeated(obj, Location.MarkedCodeProperty, result);
        }
    }
}
