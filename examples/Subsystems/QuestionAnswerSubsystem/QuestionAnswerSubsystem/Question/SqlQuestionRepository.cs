using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace QuestionAnswerSubsystem
{
    public interface IQuestionRepository : IRepository<Question>
    {

    }

    [SafeAccess]
    public class SqlQuestionRepository : SqlRepository<Question>, IQuestionRepository
    {
        private SqlQuestionRepository() { }

        public static readonly SqlQuestionRepository Instance = new SqlQuestionRepository();

    }
}
