using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;


namespace QuestionAnswerSubsystem
{
    public class CreateQuestionByMetadata : Command<Question>
    {
        private CreateQuestion _createQuestionDefinition;

        private Guid _paperMetadataId;

        public CreateQuestionByMetadata(Guid paperMetadataId, CreateQuestion createQuestionDefinition)
        {
            _paperMetadataId = paperMetadataId;
            _createQuestionDefinition = createQuestionDefinition;
        }

        protected override Question ExecuteProcedure()
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            var paper = repository.Find(_paperMetadataId, QueryLevel.Single);
            var qd = _createQuestionDefinition.Execute();
            paper.AddDefinition(qd);
            repository.Update(paper);
            return qd;
        }
    }
}
