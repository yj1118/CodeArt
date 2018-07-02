using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public static class QuestionCommon
    {
        public static Question FindBy(Guid id, QueryLevel level)
        {
            var repository = Repository.Create<IQuestionRepository>();
            return repository.Find(id, level);
        }
    }
}