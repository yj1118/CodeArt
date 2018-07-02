using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class CreatePaper : Command<Paper>
    {
        private Guid _metadataId;

        private IEnumerable<(Guid QuestionId,string Content)> _answers;

        public CreatePaper(Guid metadataId, IEnumerable<(Guid QuestionId, string Content)> answers)
        {
            _metadataId = metadataId;
            _answers = answers;
        }

        protected override Paper ExecuteProcedure()
        {
            var metadata = PaperMetadataCommon.FindBy(_metadataId, QueryLevel.None);

            var paper = new Paper(Guid.NewGuid(), metadata)
            {
                Answers = GetAnswers(_answers)
            };

            var repository = Repository.Create<IPaperRepository>();
            repository.Add(paper);
            return paper;
        }

        private static IEnumerable<Answer> GetAnswers(IEnumerable<(Guid QuestionId, string Content)> answers)
        {
            var respository = Repository.Create<IQuestionRepository>();

            var items = new List<Answer>(answers.Count());
            foreach (var arg in answers)
            {
                var question = respository.Find(arg.QuestionId, QueryLevel.None);
                var answer = new Answer(Guid.NewGuid(), question, arg.Content);
                items.Add(answer);
            }
            return items;
        }

    }
}
