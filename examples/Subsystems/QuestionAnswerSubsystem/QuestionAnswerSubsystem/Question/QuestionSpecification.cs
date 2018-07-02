using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace QuestionAnswerSubsystem
{
    [SafeAccess]
    public class QuestionSpecification : ObjectValidator<Question>
    {
        public QuestionSpecification()
        {

        }

        protected override void Validate(Question obj, ValidationResult result)
        {
            
        }
    }
}
