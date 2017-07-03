using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IPropertyValidator
    {
        void Validate(IDomainObject domainObject, IDomainProperty property, ValidationResult result);
    }
}
