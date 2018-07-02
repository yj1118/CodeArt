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
    public class AnswerSpecification : ObjectValidator<Answer>
    {
        public AnswerSpecification()
        {

        }

        protected override void Validate(Answer obj, ValidationResult result)
        {
            
        }
    }
}
