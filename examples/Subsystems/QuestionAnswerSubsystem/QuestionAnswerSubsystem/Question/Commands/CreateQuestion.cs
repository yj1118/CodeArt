using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class CreateQuestion : Command<Question>
    {
        private string _content;

        private QuestionType _type;

        private IEnumerable<string> _options;

        public CreateQuestion(string content, QuestionType type,IEnumerable<string> options)
        {
            _content = content;
            _type = type;
            _options = options;
        }

        protected override Question ExecuteProcedure()
        {
            var obj = new Question(Guid.NewGuid(), _type, _content);
            obj.Options = GetOptions(_options);

            var repository = Repository.Create<IQuestionRepository>();
            repository.Add(obj);
            return obj;
        }

        internal static IEnumerable<Option> GetOptions(IEnumerable<string> options)
        {
            var items = new List<Option>(options.Count());
            int index = 1;
            foreach (var option in options)
            {
                var item = new Option(index) { Content = option };
                items.Add(item);
                index++;
            }
            return items;
        }

    }
}
