using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace QuestionAnswerSubsystem
{
    [Event("DeletePapersEvent")]
    public class DeletePapersEvent : DomainEvent
    {
        [EventArg()]
        public Guid[] Ids
        {
            get;
            set;
        }

        public DeletePapersEvent()
        {

        }

        protected override void RaiseImplement()
        {
            var repository = Repository.Create<IPaperRepository>();
            foreach(var id in this.Ids)
            {
                var paper = repository.Find(id, QueryLevel.None);
                repository.Delete(paper);
            }
        }

        protected override void ReverseImplement()
        {
        }
    }
}
