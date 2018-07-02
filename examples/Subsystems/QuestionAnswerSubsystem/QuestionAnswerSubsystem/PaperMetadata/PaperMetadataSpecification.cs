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
    public class PaperMetadataSpecification : ObjectValidator<PaperMetadata>
    {
        public PaperMetadataSpecification()
        {

        }

        protected override void Validate(PaperMetadata obj, ValidationResult result)
        {
            Validator.CheckPropertyRepeated(obj, PaperMetadata.MarkedCodeProperty, result);
        }
    }
}
