using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class UpdateQuestion : Command<Question>
    {
        private Guid _id;

        public string Content
        {
            get;
            set;
        }

        public QuestionType? Type
        {
            get;
            set;
        }

        public IEnumerable<string> Options
        {
            get;
            set;
        }

        public UpdateQuestion(Guid id)
        {
            _id = id;
        }

        protected override Question ExecuteProcedure()
        {
            var repository = Repository.Create<IQuestionRepository>();
            var obj = repository.Find(_id, QueryLevel.Single);
            if (this.Content != null) obj.Content = this.Content;
            if (this.Type != null) obj.Type = this.Type.Value;
            if (this.Options != null) obj.Options = CreateQuestion.GetOptions(this.Options);
            repository.Update(obj);
            return obj;
        }
    }
}
