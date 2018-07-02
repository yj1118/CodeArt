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
    public class PaperSpecification : ObjectValidator<Paper>
    {
        public PaperSpecification()
        {

        }

        protected override void Validate(Paper obj, ValidationResult result)
        {
            
        }
    }
}
