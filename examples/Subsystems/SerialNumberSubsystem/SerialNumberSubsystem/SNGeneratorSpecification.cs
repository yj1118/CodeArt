using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess; 

namespace SerialNumberSubsystem
{
    [SafeAccess]
    internal sealed class SNGeneratorSpecification : ObjectValidator<SNGenerator>
    {
        public SNGeneratorSpecification()
        {
        }

        protected override void Validate(SNGenerator obj, ValidationResult result)
        {
            //唯一标示不能重复
            Validator.CheckPropertyRepeated(obj, SNGenerator.MarkedCodeProperty, result);
        }

       
    }
}
