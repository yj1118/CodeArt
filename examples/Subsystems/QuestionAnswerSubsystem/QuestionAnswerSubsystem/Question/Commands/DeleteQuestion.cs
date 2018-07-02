using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class DeleteQuestion : Command<Question>
    {
        private Guid _id;

        public DeleteQuestion(Guid id)
        {
            _id = id;
        }

        protected override Question ExecuteProcedure()
        {
            var repository = Repository.Create<IQuestionRepository>();
            var obj = repository.Find(_id, QueryLevel.None);
            repository.Delete(obj);
            return obj;
        }
    }
}
