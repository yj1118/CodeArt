using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace QuestionAnswerSubsystem
{
    public interface IAnswerRepository : IRepository<Answer>
    {

    }

    [SafeAccess]
    public class SqlAnswerRepository : SqlRepository<Answer>, IAnswerRepository
    {
        private SqlAnswerRepository() { }

        public static readonly SqlAnswerRepository Instance = new SqlAnswerRepository();
    }
}
