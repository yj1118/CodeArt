using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IObjectValidator
    {
        void Validate(IDomainObject domainObject, ValidationResult result);
    }
}