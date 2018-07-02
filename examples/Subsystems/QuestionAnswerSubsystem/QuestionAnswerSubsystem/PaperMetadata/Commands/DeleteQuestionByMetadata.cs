using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;


namespace QuestionAnswerSubsystem
{
    public class DeleteQuestionByMetadata : Command
    {
        private DeleteQuestion _deleteQuestionDefinition;

        private Guid _paperMetadataId;

        public DeleteQuestionByMetadata(Guid paperMetadataId, DeleteQuestion deleteQuestionDefinition)
        {
            _paperMetadataId = paperMetadataId;
            _deleteQuestionDefinition = deleteQuestionDefinition;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            var paper = repository.Find(_paperMetadataId, QueryLevel.Single);
            var qd = _deleteQuestionDefinition.Execute();
            paper.RemoveDefinition(qd);
            repository.Update(paper);
        }
    }
}
