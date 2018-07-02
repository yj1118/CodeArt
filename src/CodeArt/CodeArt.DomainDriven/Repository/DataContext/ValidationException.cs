using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Transactions;

using CodeArt;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    [NonLog]
    public class ValidationException : BusinessException
    {
        public ValidationResult Result
        {
            get;
            private set;
        }

        public ValidationException(ValidationResult result)
            : base(result.Message)
        {
            this.Result = result;
        }

    }
}
