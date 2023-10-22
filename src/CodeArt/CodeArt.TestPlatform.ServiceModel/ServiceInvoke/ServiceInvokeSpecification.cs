using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CodeArt;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.TestPlatform
{
    [SafeAccess]
    internal sealed class ServiceInvokeSpecification : ObjectValidator<ServiceInvoke>
    {
        public ServiceInvokeSpecification()
        {

        }

        protected override void Validate(ServiceInvoke obj, ValidationResult result)
        {
            
        } 
    }
}
