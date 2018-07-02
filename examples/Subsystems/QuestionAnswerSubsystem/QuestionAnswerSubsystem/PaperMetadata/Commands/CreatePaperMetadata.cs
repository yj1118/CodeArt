using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class CreatePaperMetadata : Command<PaperMetadata>
    {
        private string _name;

        private string _markedCoded;

        public CreatePaperMetadata(string name,string markedCoded)
        {
            _name = name;
            _markedCoded = markedCoded;
        }

        protected override PaperMetadata ExecuteProcedure()
        {
            var obj = new PaperMetadata(Guid.NewGuid(), _name, _markedCoded);
            var repository = Repository.Create<IPaperMetadataRepository>();
            repository.Add(obj);
            return obj;
        }
    }
}
