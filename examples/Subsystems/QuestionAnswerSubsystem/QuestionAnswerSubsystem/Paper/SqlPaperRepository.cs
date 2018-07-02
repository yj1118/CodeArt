using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace QuestionAnswerSubsystem
{
    public interface IPaperRepository : IRepository<Paper>
    {
    }

    [SafeAccess]
    public class SqlPaperRepository : SqlRepository<Paper>, IPaperRepository
    {
        private SqlPaperRepository() { }

        public static readonly SqlPaperRepository Instance = new SqlPaperRepository();
    }
}
