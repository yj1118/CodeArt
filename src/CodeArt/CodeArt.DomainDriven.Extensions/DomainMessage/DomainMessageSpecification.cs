using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    internal sealed class DomainMessageSpecification : ObjectValidator<DomainMessage>
    {
        public DomainMessageSpecification()
        {

        }

        protected override void Validate(DomainMessage obj, ValidationResult result)
        {

        }
    }
}