using System;
using System.Collections.Generic;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public sealed class DeletePaperMetadatas : Command
    {
        private IEnumerable<Guid> _ids;

        public DeletePaperMetadatas(IEnumerable<Guid> ids)
        {
            _ids = ids;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            foreach (var id in _ids)
            {
                var obj = repository.Find(id, QueryLevel.None);
                repository.Delete(obj);
            }
        }
    }
}
