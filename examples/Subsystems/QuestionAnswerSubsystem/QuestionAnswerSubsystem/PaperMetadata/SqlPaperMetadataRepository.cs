using System;
using System.Collections.Generic;
using CodeArt.DomainDriven;

using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace QuestionAnswerSubsystem
{
    public interface IPaperMetadataRepository : IRepository<PaperMetadata>
    {
        Page<PaperMetadata> FindPage(string name, int pageSize, int pageIndex);

        PaperMetadata FindBy(string markedCode, QueryLevel level);

    }

    [SafeAccess]
    public class SqlPaperMetadataRepository : SqlRepository<PaperMetadata>, IPaperMetadataRepository
    {
        private SqlPaperMetadataRepository() { }

        public static readonly SqlPaperMetadataRepository Instance = new SqlPaperMetadataRepository();

        public Page<PaperMetadata> FindPage(string name, int pageIndex, int pageSize)
        {
            return this.Query<PaperMetadata>("@name<name like %@name%>[order by name desc]",
                                                    pageIndex, pageSize,
                                                    (arg) =>
                                                    {
                                                        arg.TryAdd("name", name);
                                                    });
        }

        public PaperMetadata FindBy(string markedCode, QueryLevel level)
        {
            return this.QuerySingle<PaperMetadata>("markedCode=@markedCode",
                                           (arg) =>
                                           {
                                               arg.Add("markedCode", markedCode);
                                           }, level);

        }

    }
}
