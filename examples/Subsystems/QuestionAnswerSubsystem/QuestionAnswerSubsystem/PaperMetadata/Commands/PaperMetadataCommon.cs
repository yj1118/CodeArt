using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public static class PaperMetadataCommon
    {
        public static PaperMetadata FindBy(Guid id, QueryLevel level)
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            return repository.Find(id, level);
        }

        public static PaperMetadata FindByMarkedCode(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            return repository.FindBy(markedCode, level);
        }

        public static Page<PaperMetadata> FindPage(string name, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            return repository.FindPage(name, pageIndex, pageSize);
        }

    }
}