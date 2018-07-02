using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class UpdatePaperMetadata : Command<PaperMetadata>
    {
        public string Name
        {
            get;
            set;
        }

        public string MarkedCode
        {
            get;
            set;
        }

        private Guid _id;

        public UpdatePaperMetadata(Guid id)
        {
            _id = id;
        }

        protected override PaperMetadata ExecuteProcedure()
        {
            var repository = Repository.Create<IPaperMetadataRepository>();
            var obj = repository.Find(_id, QueryLevel.Single);

            if (this.Name != null) obj.Name = this.Name;
            if (this.MarkedCode != null) obj.MarkedCode = this.MarkedCode;
            repository.Update(obj);
            return obj;
        }
    }
}
