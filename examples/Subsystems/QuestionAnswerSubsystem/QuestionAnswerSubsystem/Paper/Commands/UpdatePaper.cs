using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class UpdatePaper : Command<Paper>
    {
        private Guid _id;

        private IEnumerable<(Guid Id, Guid QuestionId, string Content)> _answers; //Id是answer的编号

        public UpdatePaper(Guid id, IEnumerable<(Guid Id, Guid QuestionId, string Content)> answers)
        {
            _id = id;
            _answers = answers;
        }

        protected override Paper ExecuteProcedure()
        {
            var repository = Repository.Create<IPaperRepository>();
            var paper = repository.Find(_id, QueryLevel.Single);

            var repositoryQuestion = Repository.Create<IQuestionRepository>();

            if (_answers != null)
            {
                foreach (var arg in _answers)
                {
                    if(arg.Id == Guid.Empty)
                    {
                        var question = repositoryQuestion.Find(arg.QuestionId, QueryLevel.None);
                        var answer = new Answer(Guid.NewGuid(), question, arg.Content);
                        paper.AddAnswer(answer);
                    }
                    else
                    {
                        var answer = paper.GetAnswer(arg.Id);
                        if (answer.IsEmpty()) continue;
                        answer.Content = arg.Content;
                    }
                }
            }
            repository.Update(paper);
            return paper;
        }
    }
}
