using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeArt.DomainDriven;

namespace QuestionAnswerSubsystem
{
    public class DeletePaper : Command
    {
        private Guid _id;

        public DeletePaper(Guid id)
        {
            _id = id;
        }

        protected override void ExecuteProcedure()
        {
            var repository = Repository.Create<IPaperRepository>();
            var paper = repository.Find(_id, QueryLevel.None);
            repository.Delete(paper);
        }
    }
}
